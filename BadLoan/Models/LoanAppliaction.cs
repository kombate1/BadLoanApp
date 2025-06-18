
using System.ComponentModel.DataAnnotations;

namespace BadLoan.Models
{
    public class LoanApplication
    {
        [Key]
        public int LoanApplicationId { get; set; }

        [Required]
        public Customer CustomerId { get; set; }
  

        [Required]
        public int LoanTypeId { get; set; }

        [Required]
        public decimal RequestedAmount { get; set; }

        [Required]
        public int LoanTermYears { get; set; }

        [Required]
        public decimal TotalRepayable { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public DateTime SubmittedDate { get; set; }

        [Required]
        public DateTime LastUpdated { get; set; }
    }
}
