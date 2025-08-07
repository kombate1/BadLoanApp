// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using BadLoan.Data;
using BadLoan.Models;

//using BadLoan.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace BadLoan.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _db;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            ApplicationDbContext db,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public ProfileViewModel Input { get; set; }
        public IList<LoanApplication> loans { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            // Ensure user is not null
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Load profile from Customer table
            var profile = _db.Customers.FirstOrDefault(x => x.UserId == user.Id);
            var customerId = profile.CustomerId;
            if (profile == null)
            {
                return NotFound();
            }

            // Populate view model
            Input = new ProfileViewModel
            {
                Firstname = profile.FirstName,
                Lastname = profile.LastName,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender,
                Occupation = profile.Occupation,
                AnnualIncome = profile.AnnualIncome,
                PhoneNumber = await _userManager.GetPhoneNumberAsync(user)
            };

            loans = await _db.LoanApplications.OrderByDescending(l => l.SubmittedDate).
                 Where(l => l.CustomerId == customerId).
                 Include(l => l.LoanType).
                 ToListAsync();


            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Validate model state
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var profile = _db.Customers.FirstOrDefault(x => x.UserId == user.Id);
            if (profile == null)
            {
                return NotFound();
            }

            // Update Customer fields
            profile.FirstName = Input.Firstname;
            profile.LastName = Input.Lastname;
            profile.DateOfBirth = Input.DateOfBirth;
            profile.Gender = Input.Gender;
            profile.Occupation = Input.Occupation;
            profile.AnnualIncome = Input.AnnualIncome;
            await _db.SaveChangesAsync();

            // Update phone number via Identity
            var phoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            if (!phoneResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Unexpected error when trying to set phone number.");
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        // Profile view model for form binding
        public class ProfileViewModel
        {
            [Required]
            public string Firstname { get; set; }

            [Required]
            public string Lastname { get; set; }

            [DataType(DataType.Date)]
            public DateOnly DateOfBirth { get; set; }

            public string Gender { get; set; }

            public string Occupation { get; set; }

            public decimal AnnualIncome { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }
    }
}
