namespace BadLoan.Models.ViewModels
{
    public class LoanAttachmentViewModel
    {
        public LoanApplication? LoanApplicationDetails { get; set; }
        public string? Occupation { get; set; }
        public decimal YearlyIncome { get; set; }
        public int CustomerId { get; set; }

        //public Customer? CustomerDetails { get; set; }

        public List<UploadedDocument>? UploadedDocuments { get; set; }


    }
}
