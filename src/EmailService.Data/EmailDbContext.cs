using EmailService.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace EmailService.Data;

public class EmailDbContext : DbContext
{
    public EmailDbContext(DbContextOptions<EmailDbContext> options) : base(options)
    {
    }

    public DbSet<EmailQueue> EmailQueues { get; set; }
    public DbSet<EmailLog> EmailLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EmailQueue>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.To).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Subject).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Body).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Priority).HasMaxLength(50);
            entity.Property(e => e.EmailType).HasMaxLength(50);
            entity.Property(e => e.SourceApplication).HasMaxLength(100);
            entity.Property(e => e.CorrelationId).HasMaxLength(100);
            
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.ScheduledAt);
        });

        modelBuilder.Entity<EmailLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.To).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Subject).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.SourceApplication).HasMaxLength(100);
            entity.Property(e => e.CorrelationId).HasMaxLength(100);
            
            entity.HasOne(e => e.EmailQueue)
                  .WithMany()
                  .HasForeignKey(e => e.EmailQueueId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => e.SentAt);
            entity.HasIndex(e => e.Status);
        });
    }
}