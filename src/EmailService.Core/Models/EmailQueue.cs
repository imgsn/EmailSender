using System.ComponentModel.DataAnnotations;

namespace EmailService.Core.Models;

public class EmailQueue
{
    [Key]
    public int Id { get; set; }
    
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
    
    [Required]
    public string Body { get; set; } = string.Empty;
    
    public bool IsHtml { get; set; } = true;
    
    [MaxLength(50)]
    public string Priority { get; set; } = "Normal";
    
    [MaxLength(50)]
    public string EmailType { get; set; } = "Transactional";
    
    public string? Attachments { get; set; } // JSON array of file paths
    
    [MaxLength(50)]
    public string Status { get; set; } = "Pending";
    
    public int RetryCount { get; set; } = 0;
    
    public int MaxRetries { get; set; } = 3;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? ScheduledAt { get; set; }
    
    public DateTime? ProcessedAt { get; set; }
    
    public string? ErrorMessage { get; set; }
    
    [MaxLength(100)]
    public string? SourceApplication { get; set; }
    
    [MaxLength(100)]
    public string? CorrelationId { get; set; }
}