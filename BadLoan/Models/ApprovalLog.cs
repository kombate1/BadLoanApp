using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BadLoan.Models
{
    public class ApprovalLog
    {
        [Key]
        public int Id { get; set; }

        public string? Status { get; set; }
        public string? Comment { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public int LoanApplicationId { get; set; }
        public virtual LoanApplication LoanApplication { get; set; }

    }


}
