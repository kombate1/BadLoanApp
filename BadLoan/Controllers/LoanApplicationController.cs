using System.Threading.Tasks;
using BadLoan.Data;
using BadLoan.Models;
using BadLoan.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BadLoan.Controllers
{
    public class LoanApplicationController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public LoanApplicationController(
            ApplicationDbContext db,
            UserManager<IdentityUser> userManager
            )
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var customer = await _db.Customers.Where(c => c.UserId == user.Id).FirstOrDefaultAsync();

            LoanAttachmentViewModel viewModel = new LoanAttachmentViewModel()
            {
                LoanApplicationDetails = new LoanApplication(), 
                CustomerDetails = customer
            };

            return View(viewModel);
        }
    }
}
