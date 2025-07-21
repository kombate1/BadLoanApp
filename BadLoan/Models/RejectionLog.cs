using System.ComponentModel.DataAnnotations;

namespace BadLoan.Models
{
    public class RejectionLog
    {
            [Key]
            public int Id { get; set; }

        public decimal ApprovedAmount { get; set; }
        public int CustomerId { get; set; }
        public string? Comment { get; set; }
            public DateTime Timestamp { get; set; } = DateTime.Now;

            public int LoanApplicationId { get; set; }
            public virtual LoanApplication LoanApplication { get; set; }
        



    }

}

