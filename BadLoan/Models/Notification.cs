using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static BadLoan.Areas.Identity.Pages.Account.LoginModel;

namespace BadLoan.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId  { get; set; }
       
        public string UserId { get; set; }
        public string? Email { get; set; }
        public string? Message { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

    }
}
