using Microsoft.AspNetCore.Mvc.Rendering;

namespace BadLoan.Models.ViewModels
{
    public class LoanAttachmentViewModel
    {
        public LoanApplication? LoanApplicationDetails { get; set; }
        public string? Occupation { get; set; }
        public decimal AnnualIncome { get; set; }
        public int CustomerId { get; set; }

        public int LoanTypeId { get; set; }
        public string? LoanType { get; set; }
        public int Duration { get; set; }
        public decimal LoanAmount { get; set; }

        public List<SelectListItem> Loans { get; set; } = new List<SelectListItem>();

        public Calculator Calculation { get; set; }

        //public Customer? CustomerDetails { get; set; }

        public List<UploadedDocument>? UploadedDocuments { get; set; }


    }
}
