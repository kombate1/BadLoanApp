using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BadLoan.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        public DateOnly DateOfBirth { get; set; }
        [Required]
        public string? Gender { get; set; }
        [Required]
        public string? Occupation { get; set; }
        [Required]
        public string? EmployerName { get; set; }
        [Required]
        public string? EmployerContact { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal YearlyIncome { get; set; }
        [Required]
        public string? HomeAddress { get; set; }
        [Required]
        public string? PlaceOfWork { get; set; }

        public ICollection<LoanApplication>? LoanApplications { get; set; }

        
    }
}
