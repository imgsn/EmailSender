using EmailService.Core.Interfaces;
using EmailService.Data;
using EmailService.Data.Repositories;
using EmailService.Worker;
using EmailService.Worker.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Services.AddSerilog();

// Add Entity Framework
builder.Services.AddDbContext<EmailDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add repositories and services
builder.Services.AddScoped<IEmailRepository, EmailRepository>();
builder.Services.AddScoped<IEmailService, EmailSenderService>();

// Add background service
builder.Services.AddHostedService<EmailProcessorWorker>();

// Add health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<EmailDbContext>();

var host = builder.Build();

// Ensure database is created and migrated
using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EmailDbContext>();
    context.Database.EnsureCreated();
}

await host.RunAsync();