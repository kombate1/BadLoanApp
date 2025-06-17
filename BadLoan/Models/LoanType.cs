using System.ComponentModel.DataAnnotations;

namespace BadLoan.Models
{
    public class LoanType
    {
        [Key]
        public int LoanTypeId { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public decimal InterestRate { get; set; }

        [Required]
        public int MaxTermMonths { get; set; }

        [Required]
        public decimal MaxAmount { get; set; }

        [Required]
        public decimal MinAmount { get; set; }
    }
}
