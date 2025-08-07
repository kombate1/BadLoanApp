using BadLoan.Data;
using BadLoan.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using static BadLoan.Areas.Identity.Pages.Account.Manage.IndexModel;

namespace BadLoan.Areas.Identity.Pages.Account.Manage
{
    public class MyLoansModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _db;

        public MyLoansModel(
            UserManager<IdentityUser> userManager,
            ApplicationDbContext db,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

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

            loans = await _db.LoanApplications.Where(l => l.CustomerId == customerId).
                 Include(l => l.LoanType).
                 ToListAsync();

            return Page();

        }
    }
}
