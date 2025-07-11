using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BadLoan.Models
{
    public class LoanType
    {
        [Key]
        public int LoanTypeId { get; set; }

        [Required]
        public string LoanTypeName { get; set; }

        

        public string? Description { get; set; }

        
        public int? MaxTermMonths { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? InterestRate { get; set; }

        public decimal? MaxAmount { get; set; }

        public decimal? MinAmount { get; set; }

        public ICollection<LoanApplication> LoanApplication { get; set; }
    }
}
