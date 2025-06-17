using System.ComponentModel.DataAnnotations;

namespace BadLoan.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateOnly DateOfBirth { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string Occupation { get; set; }
        [Required]
        public string EmployerName { get; set; }
        [Required]
        public string EmployerContact { get; set; }
        [Required]
        public decimal YearlyIncome { get; set; }
        [Required]
        public string HomeAddress { get; set; }
        [Required]
        public string PlaceOfWork { get; set; }
    }
}
