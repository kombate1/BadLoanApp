using BadLoan.Models;
using BadLoan.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BadLoan.Controllers
{
    public class CalculatorController : Controller
    {
        private readonly EligibilityService _eligibilityService;

        public CalculatorController(EligibilityService eligibilityService)
        {
            _eligibilityService = eligibilityService;
        }

        public IActionResult Index()
        {
            ViewBag.MessageHtml = null;
            return View();
        }

        [HttpPost]
        public IActionResult Index(Calculator c)
        {
            if (!ModelState.IsValid)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    // Return validation errors for AJAX
                    return BadRequest(ModelState);
                }
                return View(c);
            }

            var results = _eligibilityService.LoanEligibility(c.AnnualIncome, c.Duration, c.LoanType, c.LoanAmount);

            // Set color-coded message based on eligibility
            string styledMessage;
            if (results.IsEligible) // <-- Assuming your service returns a bool property
            {
                styledMessage = $@"
                    <div class='alert alert-success mt-3'>
                        <strong>Eligible:</strong> {results.Message}
                    </div>";
            }
            else
            {
                styledMessage = $@"
                    <div class='alert alert-danger mt-3'>
                        <strong>Not Eligible:</strong> {results.Message}
                    </div>";
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Content(styledMessage, "text/html");
            }

            ViewBag.MessageHtml = styledMessage;
            return View(c);
        }
    }
}
