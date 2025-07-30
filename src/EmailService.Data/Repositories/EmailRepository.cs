using EmailService.Core.Interfaces;
using EmailService.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace EmailService.Data.Repositories;

public class EmailRepository : IEmailRepository
{
    private readonly EmailDbContext _context;

    public EmailRepository(EmailDbContext context)
    {
        _context = context;
    }

    public async Task<int> AddEmailToQueueAsync(EmailQueue emailQueue)
    {
        _context.EmailQueues.Add(emailQueue);
        await _context.SaveChangesAsync();
        return emailQueue.Id;
    }

    public async Task<List<EmailQueue>> GetPendingEmailsAsync(int batchSize = 10)
    {
        var now = DateTime.UtcNow;
        
        return await _context.EmailQueues
            .Where(e => e.Status == "Pending" && 
                       (e.ScheduledAt == null || e.ScheduledAt <= now) &&
                       e.RetryCount < e.MaxRetries)
            .OrderBy(e => e.CreatedAt)
            .Take(batchSize)
            .ToListAsync();
    }

    public async Task UpdateEmailAsync(EmailQueue emailQueue)
    {
        _context.EmailQueues.Update(emailQueue);
        await _context.SaveChangesAsync();
    }

    public async Task AddEmailLogAsync(EmailLog emailLog)
    {
        _context.EmailLogs.Add(emailLog);
        await _context.SaveChangesAsync();
    }

    public async Task<EmailQueue?> GetEmailByIdAsync(int id)
    {
        return await _context.EmailQueues.FindAsync(id);
    }
}