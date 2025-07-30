namespace EmailService.Core.Models;

public class EmailRequest
{
    public string To { get; set; } = string.Empty;
    public string? Cc { get; set; }
    public string? Bcc { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; } = true;
    public string Priority { get; set; } = "Normal";
    public string EmailType { get; set; } = "Transactional";
    public List<string>? Attachments { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public string? SourceApplication { get; set; }
    public string? CorrelationId { get; set; }
}