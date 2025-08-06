namespace BadLoan.Models.ViewModels
{
    public class AdminViewModel
    {
        public class LoanDashboardViewModel
        {
            public IEnumerable<LoanApplication> LoanApplications { get; set; }
            public IEnumerable<Notification> Notifications { get; set; }
        }
       
    }
}
