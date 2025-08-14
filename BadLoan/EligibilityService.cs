public class EligibilityService
{




    public (bool IsEligible, string Message, decimal DebtServiceRatio, decimal maxLoanAmount, decimal amountToPayMonthly) LoanEligibility(decimal annualIncome, decimal duration, string LoanType, decimal loanAmount)
    {



        decimal min = 1 * 12, max = 30 * 12;
       string loanType = LoanType?.ToLower() ?? string.Empty;

        if (loanType == null)
        {
            return (false, $"Enter the loan type before proceeding.", 0,0,0);
            //ModelState.AddModelError("Duration", $"Duration for {c.LoanType} must be between {min} and {max} years.");
        }

        switch (loanType)
        {
            case "personal":
                min = 1 * 12;
                max = 5 * 12;
                break;
            case "auto":
                min = 5 * 12;
                max = 10 * 12;
                break;
            case "mortgage":
                min = 10 * 12;
                max = 20 * 12;
                break;
        }

        if (duration < min || duration > max)
        {
            return (false, $"Duration for {loanType} must be between {min:F1} months  and {max:F1} months", 0,0,0);
            //ModelState.AddModelError("Duration", $"Duration for {c.LoanType} must be between {min} and {max} years.");
        }

        // Assuming c.AnnualIncome is a decimal and you want to calculate interest based on input from the Calculator model in the view
        decimal interest = 0;
        decimal principalInterest = 0;

        duration /= 12;

        if (LoanType == "Personal")
        {
            interest = loanAmount * 0.15m * duration; // Example calculation for personal loan
            principalInterest = 0.15m;
        }
        else if (LoanType == "Mortgage")
        {
            interest = loanAmount * 0.25m * duration; // Example calculation for home loan
            principalInterest = 0.25m;
        }
        else if (LoanType == "Auto")
        {
            interest = loanAmount * 0.20m * duration; // Example calculation for car loan
            principalInterest = 0.20m;
        }

        decimal maxLoanAmount = (annualIncome * 0.4m * duration) / (1 + (principalInterest * duration));

        decimal amountToPayYearly = (loanAmount + interest) / duration;

        decimal debtServiceRatio = (amountToPayYearly / annualIncome) * 100;

        decimal amountToPayMonthly = ((amountToPayYearly) / 12);




      //  decimal maxLoanAmount = (annualIncome * 0.4m * duration) - principalInterest; // Assuming max loan amount is 40% of annual income
        // Assuming max loan amount is 40% of annual income



        if (debtServiceRatio > 40)
        {
            return (false, $"You are <strong>not eligible</strong> for this loan because your debt ratio is <strong>{debtServiceRatio:F2}% </strong>, meaning you will be paying <strong> {debtServiceRatio:F2}%</strong> of your salary a month which is <strong>{amountToPayMonthly:F2}</strong>. Your maximum loan request amount is <strong> GH₵{maxLoanAmount:F2}</strong> ", debtServiceRatio,maxLoanAmount, amountToPayMonthly);
        }
        else
        {
            //return (true, $"Congratulations! You are <strong> eligible </strong>for this loan.Your Debt Service Ratio is <strong>{debtServiceRatio:F2}% . You will be paying <strong>{amountToPayMonthly:F2}</strong> per month", amountToPayMonthly,debtServiceRatio,maxLoanAmount);

            return (true, $"Congratulations! You are <strong> eligible </strong>for this loan.Your Debt Service Ratio is <strong>{debtServiceRatio:F2}% . You will be paying <strong>{amountToPayMonthly:F2}</strong> per year", debtServiceRatio, maxLoanAmount, amountToPayMonthly);

        }
    }


}




