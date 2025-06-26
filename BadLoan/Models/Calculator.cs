using System.ComponentModel.DataAnnotations;

namespace BadLoan.Models
{
    public class Calculator
    {
        [Range(1,10000000000)]
        public decimal MonthlyIncome { get; set; }
        [Range(1, 10000000000)]
        public decimal LoanAmount { get; set; }
        public string LoanType { get; set; }
        public int Duration { get; set; }
    }
}
