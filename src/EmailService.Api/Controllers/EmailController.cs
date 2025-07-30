using EmailService.Core.Interfaces;
using EmailService.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmailService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailController> _logger;

    public EmailController(IEmailService emailService, ILogger<EmailController> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromBody] EmailRequest emailRequest)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var emailId = await _emailService.QueueEmailAsync(emailRequest);
            
            return Ok(new { EmailId = emailId, Message = "Email queued successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error queueing email");
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }

    [HttpPost("send-bulk")]
    public async Task<IActionResult> SendBulkEmails([FromBody] List<EmailRequest> emailRequests)
    {
        try
        {
            var results = new List<object>();
            
            foreach (var emailRequest in emailRequests)
            {
                try
                {
                    var emailId = await _emailService.QueueEmailAsync(emailRequest);
                    results.Add(new { EmailId = emailId, Status = "Queued", To = emailRequest.To });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error queueing email to {To}", emailRequest.To);
                    results.Add(new { EmailId = (int?)null, Status = "Failed", To = emailRequest.To, Error = ex.Message });
                }
            }
            
            return Ok(new { Results = results });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing bulk emails");
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }

    [HttpGet("status/{emailId}")]
    public async Task<IActionResult> GetEmailStatus(int emailId)
    {
        try
        {
            // This would require adding a method to get email status
            // For now, returning a placeholder
            return Ok(new { EmailId = emailId, Status = "Processing" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting email status for ID {EmailId}", emailId);
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }
}