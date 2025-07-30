# Email Service

A standalone email sending service built with .NET 8 and SQL Server that processes emails from a queue with comprehensive logging.

## Features

- **Queue-based email processing** - Emails are queued and processed asynchronously
- **Retry mechanism** - Failed emails are retried with configurable limits
- **Comprehensive logging** - All email activities are logged to database and files
- **REST API** - Simple API for other applications to queue emails
- **Background service** - Runs as a Windows Service or console application
- **Database integration** - Uses SQL Server with Entity Framework Core
- **Docker support** - Containerized deployment
- **Health checks** - Built-in health monitoring

## Architecture

- **EmailService.Core** - Domain models and interfaces
- **EmailService.Data** - Data access layer with Entity Framework
- **EmailService.Worker** - Background service for email processing
- **EmailService.Api** - REST API for email queueing

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB, Express, or Full)
- SMTP server credentials

### Configuration

1. Update `appsettings.json` with your database connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your SQL Server connection string"
  }
}
```

2. Configure SMTP settings:
```json
{
  "SmtpSettings": {
    "Server": "smtp.gmail.com",
    "Port": 587,
    "EnableSsl": true,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "FromEmail": "your-email@gmail.com"
  }
}
```

### Running the Service

1. **Database Setup**:
```bash
cd src/EmailService.Worker
dotnet ef database update
```

2. **Start the Worker Service**:
```bash
cd src/EmailService.Worker
dotnet run
```

3. **Start the API** (optional):
```bash
cd src/EmailService.Api
dotnet run
```

### Using the API

Queue a single email:
```bash
POST /api/email/send
{
  "to": "recipient@example.com",
  "subject": "Test Email",
  "body": "<h1>Hello World</h1>",
  "isHtml": true,
  "priority": "Normal",
  "emailType": "Transactional"
}
```

Queue multiple emails:
```bash
POST /api/email/send-bulk
[
  {
    "to": "user1@example.com",
    "subject": "Newsletter",
    "body": "Newsletter content"
  },
  {
    "to": "user2@example.com",
    "subject": "Newsletter",
    "body": "Newsletter content"
  }
]
```

## Database Schema

### EmailQueues Table
- Stores emails to be sent
- Tracks retry counts and status
- Supports scheduled sending

### EmailLogs Table
- Logs all email sending attempts
- Tracks success/failure with error messages
- Provides audit trail for compliance

## Deployment

### Windows Service
```bash
dotnet publish -c Release
sc create EmailService binPath="path\to\EmailService.Worker.exe"
sc start EmailService
```

### Docker
```bash
docker-compose up -d
```

## Monitoring

- Health checks available at `/health`
- Structured logging with Serilog
- Email metrics in database logs
- File-based logging for troubleshooting

## Configuration Options

- **Batch size** - Number of emails processed per cycle
- **Retry limits** - Maximum retry attempts for failed emails
- **Processing interval** - Delay between processing cycles
- **SMTP settings** - Server, authentication, and security settings

## Integration Examples

### C# Client
```csharp
var emailRequest = new EmailRequest
{
    To = "user@example.com",
    Subject = "Welcome",
    Body = "Welcome to our service!",
    SourceApplication = "MyWebApp"
};

var response = await httpClient.PostAsJsonAsync("/api/email/send", emailRequest);
```

### Direct Database Insert
```sql
INSERT INTO EmailQueues (To, Subject, Body, SourceApplication)
VALUES ('user@example.com', 'Test', 'Test message', 'DirectSQL');
```