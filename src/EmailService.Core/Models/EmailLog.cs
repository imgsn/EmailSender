using System.ComponentModel.DataAnnotations;

namespace EmailService.Core.Models;

public class EmailLog
{
    [Key]
    public int Id { get; set; }
    
    public int EmailQueueId { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string To { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Cc { get; set; }
    
    [MaxLength(500)]
    public string? Bcc { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Subject { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;
    
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    
    public string? ErrorMessage { get; set; }
    
    public int RetryAttempt { get; set; }
    
    [MaxLength(100)]
    public string? SourceApplication { get; set; }
    
    [MaxLength(100)]
    public string? CorrelationId { get; set; }
    
    public virtual EmailQueue EmailQueue { get; set; } = null!;
}