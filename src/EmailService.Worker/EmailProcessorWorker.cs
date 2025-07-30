using EmailService.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EmailService.Worker;

public class EmailProcessorWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmailProcessorWorker> _logger;
    private readonly int _batchSize;
    private readonly int _delayMilliseconds;

    public EmailProcessorWorker(
        IServiceProvider serviceProvider,
        ILogger<EmailProcessorWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _batchSize = 10; // Process 10 emails at a time
        _delayMilliseconds = 5000; // 5 seconds delay between batches
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Email Processor Worker started at: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                var pendingEmails = await emailService.GetPendingEmailsAsync(_batchSize);

                if (pendingEmails.Any())
                {
                    _logger.LogInformation("Processing {Count} pending emails", pendingEmails.Count);

                    var tasks = pendingEmails.Select(async email =>
                    {
                        try
                        {
                            await emailService.SendEmailAsync(email);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error processing email {EmailId}", email.Id);
                        }
                    });

                    await Task.WhenAll(tasks);
                }
                else
                {
                    _logger.LogDebug("No pending emails found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in email processing cycle");
            }

            await Task.Delay(_delayMilliseconds, stoppingToken);
        }

        _logger.LogInformation("Email Processor Worker stopped at: {time}", DateTimeOffset.Now);
    }
}