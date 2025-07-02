namespace BadLoan.Models.ViewModels
{
    public class LoanAttachmentViewModel
    {
        public LoanApplication? LoanApplicationDetails { get; set; }

        public Customer CustomerDetails { get; set; }

        public List<UploadedDocument>? UploadedDocuments { get; set; }


    }
}
