using EmailService.Core.Models;

namespace EmailService.Core.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(EmailQueue emailQueue);
    Task<int> QueueEmailAsync(EmailRequest emailRequest);
    Task<List<EmailQueue>> GetPendingEmailsAsync(int batchSize = 10);
    Task UpdateEmailStatusAsync(int emailId, string status, string? errorMessage = null);
    Task LogEmailAsync(EmailQueue emailQueue, string status, string? errorMessage = null, int retryAttempt = 0);
}