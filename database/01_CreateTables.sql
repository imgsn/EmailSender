-- Create EmailQueues table
CREATE TABLE [dbo].[EmailQueues] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [To] nvarchar(255) NOT NULL,
    [Cc] nvarchar(500) NULL,
    [Bcc] nvarchar(500) NULL,
    [Subject] nvarchar(255) NOT NULL,
    [Body] nvarchar(max) NOT NULL,
    [IsHtml] bit NOT NULL DEFAULT 1,
    [Priority] nvarchar(50) NOT NULL DEFAULT 'Normal',
    [EmailType] nvarchar(50) NOT NULL DEFAULT 'Transactional',
    [Attachments] nvarchar(max) NULL,
    [Status] nvarchar(50) NOT NULL DEFAULT 'Pending',
    [RetryCount] int NOT NULL DEFAULT 0,
    [MaxRetries] int NOT NULL DEFAULT 3,
    [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
    [ScheduledAt] datetime2 NULL,
    [ProcessedAt] datetime2 NULL,
    [ErrorMessage] nvarchar(max) NULL,
    [SourceApplication] nvarchar(100) NULL,
    [CorrelationId] nvarchar(100) NULL,
    CONSTRAINT [PK_EmailQueues] PRIMARY KEY ([Id])
);

-- Create EmailLogs table
CREATE TABLE [dbo].[EmailLogs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [EmailQueueId] int NOT NULL,
    [To] nvarchar(255) NOT NULL,
    [Cc] nvarchar(500) NULL,
    [Bcc] nvarchar(500) NULL,
    [Subject] nvarchar(255) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [SentAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
    [ErrorMessage] nvarchar(max) NULL,
    [RetryAttempt] int NOT NULL DEFAULT 0,
    [SourceApplication] nvarchar(100) NULL,
    [CorrelationId] nvarchar(100) NULL,
    CONSTRAINT [PK_EmailLogs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EmailLogs_EmailQueues] FOREIGN KEY ([EmailQueueId]) REFERENCES [dbo].[EmailQueues] ([Id]) ON DELETE CASCADE
);

-- Create indexes for better performance
CREATE INDEX [IX_EmailQueues_Status] ON [dbo].[EmailQueues] ([Status]);
CREATE INDEX [IX_EmailQueues_CreatedAt] ON [dbo].[EmailQueues] ([CreatedAt]);
CREATE INDEX [IX_EmailQueues_ScheduledAt] ON [dbo].[EmailQueues] ([ScheduledAt]);
CREATE INDEX [IX_EmailLogs_SentAt] ON [dbo].[EmailLogs] ([SentAt]);
CREATE INDEX [IX_EmailLogs_Status] ON [dbo].[EmailLogs] ([Status]);