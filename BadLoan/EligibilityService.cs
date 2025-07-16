using BadLoan.Data;
using BadLoan.Models;
using BadLoan.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
public class EligibilityService
    {

    
    

    public (bool IsEligible, string Message, decimal DebtServiceRatio) LoanEligibility(decimal annualIncome, int duration, string LoanType, decimal loanAmount)
        {
      

            int min = 1, max = 30;
            string loanType = LoanType?.ToLower() ?? string.Empty;

        if (loanType == null)
        {
            return (false, $"Enter the loan type before proceeding.", 0);
            //ModelState.AddModelError("Duration", $"Duration for {c.LoanType} must be between {min} and {max} years.");
        }

        switch (loanType)
            {
                case "personal":
                min = 1;
                    max = 5;
                    break;
                case "auto":
                min = 5;
                    max = 10;
                    break;
                case "mortgage":
                    min = 10;
                    max = 20;
                    break;
            }

            if (duration < min || duration > max)
            {
            return (false,$"Duration for {loanType} must be between {min} and {max}", 0);
                //ModelState.AddModelError("Duration", $"Duration for {c.LoanType} must be between {min} and {max} years.");
            }

            // Assuming c.AnnualIncome is a decimal and you want to calculate interest based on input from the Calculator model in the view
            decimal interest = 0;

            if (LoanType == "Personal")
            {
                interest = loanAmount * 0.15m * duration; // Example calculation for personal loan
            }
            else if (LoanType == "Home")
            {
                interest = loanAmount * 0.25m * duration; // Example calculation for home loan
            }
            else if (LoanType == "Auto")
            {
                interest = loanAmount * 0.20m * duration; // Example calculation for car loan
            }

            decimal amountToPayYearly = (loanAmount / duration) + interest;

            decimal debtServiceRatio = (amountToPayYearly / annualIncome) * 100;

            decimal maxLoanAmount = annualIncome * 0.4m; // Assuming max loan amount is 40% of annual income

            if (debtServiceRatio > 40)
            {
                return (false, $"You are <strong>not eligible</strong> for this loan because your debt ratio is <strong>{debtServiceRatio:F2}% </strong>, meaning you will be paying <strong> {debtServiceRatio:F2}%</strong> of your salary. Your maximum loan request amount is <strong> GH₵{maxLoanAmount:F2}</strong> ", debtServiceRatio);
            }
            else
            {
                return (true, "Congratulations! You are <strong> eligible </strong>for this loan.", debtServiceRatio);
            }
        }

    
}


    

