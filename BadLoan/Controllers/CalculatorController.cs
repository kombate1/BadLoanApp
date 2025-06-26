
using BadLoan.Models;
using Microsoft.AspNetCore.Mvc;


namespace BadLoan.Controllers
{
    public class CalculatorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Calculator c)
        {
            // Assuming c.AnnualIncome is a decimal and you want to calculate interest based on input from the Calculator model in the view
            decimal interest = 0;

            if(c.LoanType == "Personal")
            {
                interest = c.LoanAmount * 0.15m; // Example calculation for personal loan
            }
            else if(c.LoanType == "Home")
            {
                interest = c.LoanAmount * 0.25m; // Example calculation for home loan
            }
            else if(c.LoanType == "Auto")
            {
                interest = c.LoanAmount * 0.20m; // Example calculation for car loan
            }

            decimal amountToPayMthly = (c.LoanAmount / c.Duration) + interest;

            decimal debtServiceRatio = (amountToPayMthly / c.MonthlyIncome) * 100;

            ViewBag.Message = debtServiceRatio;

            if (debtServiceRatio > 40)
            {
                ViewBag.Message = "You are not eligible for this loan";
            }
            else
            {
                ViewBag.Message = "You are eligible for this loan";
            }

            return View(c);
        }

        
    }
}
