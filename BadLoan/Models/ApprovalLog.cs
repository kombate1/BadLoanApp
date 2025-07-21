using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BadLoan.Models
{
    public class ApprovalLog
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string? Status { get; set; }
        public string? Comment { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public decimal ApprovedAmount { get; set; }



        public int LoanApplicationId { get; set; }
        public virtual LoanApplication LoanApplication { get; set; }

    }


}
