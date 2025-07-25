using BadLoan.Data;
using BadLoan.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Linq;

namespace BadLoan.Controllers
{
    public class AdminController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.countAllLoans = await _db.LoanApplications.CountAsync();
            ViewBag.countPending = await _db.LoanApplications.CountAsync(l => l.Status == "pending");
            ViewBag.countApproved = await _db.LoanApplications.CountAsync(l => l.Status == "approved");
            ViewBag.countRejected = await _db.LoanApplications.CountAsync(l => l.Status == "rejected");
            ViewBag.totalDisbursed = await _db.LoanApplications.
                Where(l => l.Status == "approved").
                SumAsync(l => l.LoanAmount);


            //ViewBag.Name = _userManager.GetUserNameAsync(User)

            return View();
        }


        public async Task<IActionResult> GetAllLoans()
        {

            var getLoans = await _db.LoanApplications.
                OrderByDescending(l => l.SubmittedDate).
                Include(l => l.LoanType).
                Include(l => l.Customer).
                Include(l => l.UploadedDocuments).
                ToListAsync();

            int countAllLoan = getLoans.Count;
            ViewBag.countAllLoans = countAllLoan;

            return View(getLoans);
        }
        public async Task<IActionResult> Details(int id)
        {
            var getLoans = await _db.LoanApplications
                .Include(l => l.LoanType)
                .Include(l => l.Customer)
                .Include(l => l.UploadedDocuments)
                .Where(l => l.Id == id)
                .ToListAsync();

            if (getLoans == null || getLoans.Count == 0)
            {
                return NotFound();
            }

            return View(getLoans);
        }


        [HttpGet]
        public async Task<IActionResult> ApproveApplication(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var application = await _db.LoanApplications.FindAsync(id);

            if (application == null)
            {
                return NotFound();
            }

            //var alreadyApproved = await _db.

            // Example: update status to "Approved"
            application.Status = "Approved";
            application.LastUpdated = DateTime.UtcNow;

            _db.LoanApplications.Update(application);

            var approved = new ApprovalLog
            {
                CustomerId = application.CustomerId,
                LoanApplicationId = application.Id,
                ApprovedAmount = application.LoanAmount,
            };

            await _db.ApprovalLogs.AddAsync(approved);
            await _db.SaveChangesAsync();


            TempData["Message"] = "Application approved successfully.";
            // Redirect or return confirmation view
            return RedirectToAction("Index", "Admin"); // Or wherever makes sense
        }

        [HttpGet]
        public async Task<IActionResult> RejectApplication(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var application = await _db.LoanApplications.FindAsync(id);

            if (application == null)
            {
                return NotFound();
            }

            // Example: update status to "Approved"
            application.Status = "Rejected";
            application.LastUpdated = DateTime.UtcNow;
            _db.LoanApplications.Update(application);

            var rejected = new RejectionLog
            {
                CustomerId = application.CustomerId,
                LoanApplicationId = application.Id,
                ApprovedAmount = application.LoanAmount,
            };

            await _db.RejectionLogs.AddAsync(rejected);
            await _db.SaveChangesAsync();

            // Redirect or return confirmation view
            return RedirectToAction("Index", "Admin"); // Or wherever makes sense
        }




        [HttpGet]
        public async Task<IActionResult> GetApprovedLoans()
        {

            var approvedLoan = await _db.LoanApplications.
                Include(l => l.LoanType).
                Include(l => l.Customer).
                Include(l => l.UploadedDocuments).
                Where(l => l.Status == "approved").
                ToListAsync();

            int countApproved = approvedLoan.Count;
            ViewBag.countApproved = countApproved;

            return View(approvedLoan);
        }

        public async Task<IActionResult> GetRejectedLoans()
        {

            var rejectedLoan = await _db.LoanApplications.
                Include(l => l.LoanType).
                Include(l => l.Customer).
                Include(l => l.UploadedDocuments).
                Where(l => l.Status.ToLower() == "rejected").
                ToListAsync();

            int countRejected = rejectedLoan.Count;

            ViewBag.countRejected = countRejected;



            return View(rejectedLoan);


        }

        public async Task<IActionResult> GetPendingLoans()
        {

            var pendingLoan = await _db.LoanApplications.
                Include(l => l.LoanType).
                Include(l => l.Customer).
                Include(l => l.UploadedDocuments).
                Where(l => l.Status == "pending").
                ToListAsync();

            int countPending = pendingLoan.Count;
            ViewBag.countPending = countPending;

            return View(pendingLoan);
        }


        [HttpPost]
        public async Task<IActionResult> ApprovalMessage(ApprovalLog obj, int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var application = await _db.LoanApplications.FindAsync(id);

            if (application == null)
            {
                return NotFound();
            }

            var approved = new ApprovalLog
            {
                Comment = obj.Comment,
            };

            await _db.ApprovalLogs.AddAsync(approved);
            await _db.SaveChangesAsync();

            return View(approved);



        }
    }
}
