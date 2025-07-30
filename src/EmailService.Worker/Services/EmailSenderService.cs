using EmailService.Core.Interfaces;
using EmailService.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Text.Json;

namespace EmailService.Worker.Services;

public class EmailSenderService : IEmailService
{
    private readonly IEmailRepository _emailRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailSenderService> _logger;

    public EmailSenderService(
        IEmailRepository emailRepository,
        IConfiguration configuration,
        ILogger<EmailSenderService> logger)
    {
        _emailRepository = emailRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(EmailQueue emailQueue)
    {
        try
        {
            using var smtpClient = CreateSmtpClient();
            using var mailMessage = CreateMailMessage(emailQueue);

            await smtpClient.SendMailAsync(mailMessage);
            
            await UpdateEmailStatusAsync(emailQueue.Id, "Sent");
            await LogEmailAsync(emailQueue, "Sent", null, emailQueue.RetryCount);
            
            _logger.LogInformation("Email sent successfully. ID: {EmailId}, To: {To}", 
                emailQueue.Id, emailQueue.To);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email. ID: {EmailId}, To: {To}", 
                emailQueue.Id, emailQueue.To);
            
            emailQueue.RetryCount++;
            var status = emailQueue.RetryCount >= emailQueue.MaxRetries ? "Failed" : "Pending";
            
            await UpdateEmailStatusAsync(emailQueue.Id, status, ex.Message);
            await LogEmailAsync(emailQueue, "Failed", ex.Message, emailQueue.RetryCount);
            
            return false;
        }
    }

    public async Task<int> QueueEmailAsync(EmailRequest emailRequest)
    {
        var emailQueue = new EmailQueue
        {
            To = emailRequest.To,
            Cc = emailRequest.Cc,
            Bcc = emailRequest.Bcc,
            Subject = emailRequest.Subject,
            Body = emailRequest.Body,
            IsHtml = emailRequest.IsHtml,
            Priority = emailRequest.Priority,
            EmailType = emailRequest.EmailType,
            Attachments = emailRequest.Attachments != null ? 
                JsonSerializer.Serialize(emailRequest.Attachments) : null,
            ScheduledAt = emailRequest.ScheduledAt,
            SourceApplication = emailRequest.SourceApplication,
            CorrelationId = emailRequest.CorrelationId ?? Guid.NewGuid().ToString()
        };

        var emailId = await _emailRepository.AddEmailToQueueAsync(emailQueue);
        
        _logger.LogInformation("Email queued successfully. ID: {EmailId}, To: {To}", 
            emailId, emailRequest.To);
        
        return emailId;
    }

    public async Task<List<EmailQueue>> GetPendingEmailsAsync(int batchSize = 10)
    {
        return await _emailRepository.GetPendingEmailsAsync(batchSize);
    }

    public async Task UpdateEmailStatusAsync(int emailId, string status, string? errorMessage = null)
    {
        var email = await _emailRepository.GetEmailByIdAsync(emailId);
        if (email != null)
        {
            email.Status = status;
            email.ErrorMessage = errorMessage;
            email.ProcessedAt = DateTime.UtcNow;
            
            await _emailRepository.UpdateEmailAsync(email);
        }
    }

    public async Task LogEmailAsync(EmailQueue emailQueue, string status, string? errorMessage = null, int retryAttempt = 0)
    {
        var emailLog = new EmailLog
        {
            EmailQueueId = emailQueue.Id,
            To = emailQueue.To,
            Cc = emailQueue.Cc,
            Bcc = emailQueue.Bcc,
            Subject = emailQueue.Subject,
            Status = status,
            ErrorMessage = errorMessage,
            RetryAttempt = retryAttempt,
            SourceApplication = emailQueue.SourceApplication,
            CorrelationId = emailQueue.CorrelationId
        };

        await _emailRepository.AddEmailLogAsync(emailLog);
    }

    private SmtpClient CreateSmtpClient()
    {
        var smtpSettings = _configuration.GetSection("SmtpSettings");
        
        var smtpClient = new SmtpClient(smtpSettings["Server"])
        {
            Port = int.Parse(smtpSettings["Port"] ?? "587"),
            EnableSsl = bool.Parse(smtpSettings["EnableSsl"] ?? "true"),
            Credentials = new NetworkCredential(
                smtpSettings["Username"],
                smtpSettings["Password"])
        };

        return smtpClient;
    }

    private MailMessage CreateMailMessage(EmailQueue emailQueue)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_configuration["SmtpSettings:FromEmail"] ?? "noreply@example.com"),
            Subject = emailQueue.Subject,
            Body = emailQueue.Body,
            IsBodyHtml = emailQueue.IsHtml
        };

        // Add recipients
        mailMessage.To.Add(emailQueue.To);
        
        if (!string.IsNullOrEmpty(emailQueue.Cc))
        {
            mailMessage.CC.Add(emailQueue.Cc);
        }
        
        if (!string.IsNullOrEmpty(emailQueue.Bcc))
        {
            mailMessage.Bcc.Add(emailQueue.Bcc);
        }

        // Set priority
        mailMessage.Priority = emailQueue.Priority.ToLowerInvariant() switch
        {
            "high" => MailPriority.High,
            "low" => MailPriority.Low,
            _ => MailPriority.Normal
        };

        // Add attachments if any
        if (!string.IsNullOrEmpty(emailQueue.Attachments))
        {
            try
            {
                var attachmentPaths = JsonSerializer.Deserialize<List<string>>(emailQueue.Attachments);
                if (attachmentPaths != null)
                {
                    foreach (var path in attachmentPaths.Where(File.Exists))
                    {
                        mailMessage.Attachments.Add(new Attachment(path));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to add attachments for email {EmailId}", emailQueue.Id);
            }
        }

        return mailMessage;
    }
}