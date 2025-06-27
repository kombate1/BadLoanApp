
using BadLoan.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;


namespace BadLoan.Controllers
{
    public class CalculatorController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.MessageHtml = null;
            return View();
        }


        

        [HttpPost]
        public IActionResult Index(Calculator c)
        {

            // Server-side duration validation based on loan type
            int min = 1, max = 30;
            switch (c.LoanType?.ToLower())
            {
                case "personal":
                    max = 5;
                    break;
                case "auto":
                    max = 7;
                    break;
                case "mortgage":
                    min = 5;
                    max = 30;
                    break;
            }

            if (c.Duration < min ||c.Duration > max)
            {
                ModelState.AddModelError("Duration", $"Duration for {c.LoanType} must be between {min} and {max} years.");
                //ModelState.AddModelError("Duration", $"Duration for {c.LoanType} must be between {min} and {max} years.");
            }

            if (!ModelState.IsValid)
            {
                return View(c);
            }

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
                ViewBag.MessageHtml = @"
        <div class='card text-white bg-danger mt-3'>
            <div class='card-header'>Loan Status</div>
            <div class='card-body'>
                <h5 class='card-title'>Not Eligible</h5>
                <p class='card-text'>You are not eligible for this loan. Please reduce your loan request amount.</p>
            </div>
        </div>";
            }
            else
            {
                ViewBag.MessageHtml = @"
        <div class='card text-white bg-success mt-3'>
            <div class='card-header'>Loan Status</div>
            <div class='card-body'>
                <h5 class='card-title'>Eligible</h5>
                <p class='card-text'>Congratulations! You are eligible for this loan.</p>
            </div>
        </div>";
            }

           


            //if (debtServiceRatio > 40)
            //{
            //    ViewBag.Message = "You are not eligible for this loan";
            //}
            //else
            //{
            //    ViewBag.Message = "You are eligible for this loan";
            //}

            return View(c);
        }

        
    }
}
