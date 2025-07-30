using EmailService.Core.Models;

namespace EmailService.Core.Interfaces;

public interface IEmailRepository
{
    Task<int> AddEmailToQueueAsync(EmailQueue emailQueue);
    Task<List<EmailQueue>> GetPendingEmailsAsync(int batchSize = 10);
    Task UpdateEmailAsync(EmailQueue emailQueue);
    Task AddEmailLogAsync(EmailLog emailLog);
    Task<EmailQueue?> GetEmailByIdAsync(int id);
}