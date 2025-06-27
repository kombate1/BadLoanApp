using Microsoft.AspNetCore.Mvc;
using BadLoan.Models;

namespace BadLoan.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoanApplication model)
        {
            if (ModelState.IsValid)
            {
                // You can save model to DB or Session here
                return RedirectToAction("Success");
            }

            return View(model); // show form with validation messages
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
