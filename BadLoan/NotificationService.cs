using BadLoan.Data;
using BadLoan.Models;
using System.Net.Mail;

namespace BadLoan
{
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public NotificationService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task CreateNotification(string userId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task SendEmailNotification(string toEmail, string subject, string body)
        {
            var smtpSection = _configuration.GetSection("Smtp");
            var host = smtpSection["Host"];
            var port = int.Parse(smtpSection["Port"]);
            var username = smtpSection["Username"];
            var password = smtpSection["Password"];
            var fromEmail = smtpSection["From"];

            using var client = new SmtpClient(host, port)
            {
                Credentials = new System.Net.NetworkCredential(username, password),
                EnableSsl = true
            };
            var mail = new MailMessage(fromEmail, toEmail, subject, body);
            await client.SendMailAsync(mail);
        }

        //public async Task<List<Notification>> GetUnreadNotifications(string userId)
        //{
        //    return await _context.Notifications
        //        .Where(n => n.UserId == userId && !n.IsRead)
        //        .OrderByDescending(n => n.CreatedAt)
        //        .ToListAsync();
        //}
    }

}
