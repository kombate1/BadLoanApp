using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BadLoan.Models
{
    public class ApprovalLog
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("LoanApplication")]
        public string Status { get; set; }
        public string Comment { get; set; }
        public DateTime Timestamp { get; set; }= DateTime.UtcNow;
    }
}
