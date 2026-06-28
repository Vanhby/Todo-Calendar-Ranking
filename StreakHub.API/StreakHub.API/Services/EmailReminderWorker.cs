using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using StreakHub.API.Data;
using StreakHub.API.Interfaces;

namespace StreakHub.API.Services
{
    public class EmailReminderWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EmailReminderWorker> _logger;
        private readonly IConfiguration _configuration;

        public EmailReminderWorker(
            IServiceProvider serviceProvider,
            ILogger<EmailReminderWorker> logger,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("EmailReminderWorker background task is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessRemindersAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing reminders in EmailReminderWorker.");
                }

                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }

        private async Task ProcessRemindersAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var dndService = scope.ServiceProvider.GetRequiredService<IDndService>();

            var now = DateTime.UtcNow;
            var pendingReminders = await context.Reminders
                .Include(r => r.Todo)
                .ThenInclude(t => t.User)
                .Where(r => r.NotifyTime <= now && !r.IsSent)
                .ToListAsync();

            if (pendingReminders.Count == 0) return;

            foreach (var reminder in pendingReminders)
            {
                var todo = reminder.Todo;
                if (todo == null) continue;

                var user = todo.User;
                if (user == null) continue;
                if (await dndService.IsUserInDndAsync(user.UserId))
                {
                    _logger.LogInformation($"Skipped sending email reminder {reminder.Id} because User {user.UserId} is in DND.");
                    continue;
                }

                bool success = await SendEmailAsync(user.Email, todo.Title, reminder.NotifyTime);
                if (success)
                {
                    reminder.IsSent = true;
                }
            }

            await context.SaveChangesAsync();
        }

        private async Task<bool> SendEmailAsync(string recipientEmail, string todoTitle, DateTime notifyTime)
        {
            var smtpSection = _configuration.GetSection("SmtpSettings");
            var server = smtpSection.GetValue<string>("Host") ?? smtpSection.GetValue<string>("Server") ?? "";
            var port = smtpSection.GetValue<int>("Port", 587);
            var senderEmail = smtpSection.GetValue<string>("SenderEmail") ?? "";
            var senderName = smtpSection.GetValue<string>("SenderName") ?? "StreakHub Reminders";
            var username = smtpSection.GetValue<string>("Username") ?? "";
            var password = smtpSection.GetValue<string>("Password") ?? "";
            var enableSsl = smtpSection.GetValue<bool>("EnableSsl", true);

            var subject = "Reminder Notification";
            var body = $"- Reminder title: {todoTitle}\n- Reminder time: {notifyTime.ToLocalTime():yyyy-MM-dd HH:mm:ss}\n- Reminder content: You have an upcoming task: {todoTitle}";

            _logger.LogInformation($"[Email Simulation] Outbox to {recipientEmail}:\nSubject: {subject}\nBody:\n{body}");

            if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning("Gmail SMTP settings not configured or incomplete. Simulated email output to logs succeeded.");
                return true;
            }

            try
            {
                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                };
                mailMessage.To.Add(recipientEmail);

                using var smtpClient = new SmtpClient(server, port)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = enableSsl
                };

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent successfully to {recipientEmail} via SMTP.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {recipientEmail} via Gmail SMTP.");
                return false;
            }
        }
    }
}
