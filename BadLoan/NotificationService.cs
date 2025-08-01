using BadLoan.Data;
using BadLoan.Models;

namespace BadLoan
{
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateNotification(int userId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
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
