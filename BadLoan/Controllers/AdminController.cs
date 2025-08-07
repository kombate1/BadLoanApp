using BadLoan.Data;
using BadLoan.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Linq;

namespace BadLoan.Controllers
{
    [Authorize(Roles = "Approval Manager")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly NotificationService _notificationService;

        public AdminController(ApplicationDbContext db, UserManager<IdentityUser> userManager, NotificationService notificationService)
        {
            _db = db;
            _userManager = userManager;
            _notificationService = notificationService;
        }


        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }


            ViewBag.countAllLoans = await _db.LoanApplications.CountAsync();
            ViewBag.countPending = await _db.LoanApplications.CountAsync(l => l.Status == "pending");
            ViewBag.countApproved = await _db.LoanApplications.CountAsync(l => l.Status == "approved");
            ViewBag.countRejected = await _db.LoanApplications.CountAsync(l => l.Status == "rejected");

            ViewBag.totalDisbursed = await _db.LoanApplications.
                Where(l => l.Status == "approved").
                SumAsync(l => l.LoanAmount);
            var user = await _userManager.FindByNameAsync(User!.Identity!.Name!);
            if (user == null)
            {
                return Unauthorized();
            }

            var userID = user.Id.ToString();


            var notifications = await  _db.Notifications.Where(n => n.UserId == userID && !n.IsRead).ToListAsync();

            

            //ViewBag.Name = _userManager.GetUserNameAsync(User)

           


            return View(notifications);
        }

        public async Task<IActionResult> GetAllLoans()
        {
            var getLoans = await _db.LoanApplications
                .OrderByDescending(l => l.SubmittedDate)
                .Include(l => l.LoanType)
                .Include(l => l.Customer)
                .Include(l => l.UploadedDocuments)
                .ToListAsync();

            ViewBag.countAllLoans = getLoans.Count;
            return View(getLoans);
        }

        public IActionResult Details(int id)
        {
            var loan = _db.LoanApplications
                .Include(l => l.Customer)
                .Include(l => l.LoanType).
                    Include(l => l.UploadedDocuments)
                .FirstOrDefault(l => l.Id == id);

            if (loan == null)
            {
                return NotFound();
            }

            return View(loan); // Not a list!
        }

        [HttpGet]
        public async Task<IActionResult> LoadLoanDetailsPartial(int id)
        {
            var loan = await _db.LoanApplications
                .Include(l => l.Customer)
                .Include(l => l.LoanType)
                .Include(l => l.UploadedDocuments)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null)
                return NotFound();

            return PartialView("~/Views/Shared/_LoanDetailsPartial.cshtml", loan);
        }

        [HttpGet]
        public async Task<IActionResult> ApproveApplication(int id)
        {
            if (id == 0)
                return NotFound();

            var application = await _db.LoanApplications.FindAsync(id);
            if (application == null)
                return NotFound();

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

            var user = await _db.Customers.FirstOrDefaultAsync(c => c.CustomerId == application.CustomerId);
            var UserID = user.UserId;

            await _notificationService.CreateNotification(
                UserID,
                $"Dear {user.FirstName} {user.LastName}, your loan application with ID #{application.Id} has been {application.Status}");

            await _db.SaveChangesAsync();

            TempData["Message"] = "Application approved successfully.";
            // Redirect or return confirmation view
            return RedirectToAction("Index", "Admin"); // Or wherever makes sense
        }

        [HttpGet]
        public async Task<IActionResult> RejectApplication(int id, string comment)
        {
            if (id == 0)
                return NotFound();

            var application = await _db.LoanApplications.FindAsync(id);
            if (application == null)
                return NotFound();

            application.Status = "Rejected";
            application.LastUpdated = DateTime.UtcNow;

            _db.LoanApplications.Update(application);

            var rejected = new RejectionLog
            {
                CustomerId = application.CustomerId,
                LoanApplicationId = application.Id,
                ApprovedAmount = application.LoanAmount,
                Comment = comment
            };

            await _db.RejectionLogs.AddAsync(rejected);

            var user = await _db.Customers.FirstOrDefaultAsync(c => c.CustomerId == application.CustomerId);
            var UserID = user.UserId;

            await _notificationService.CreateNotification(
                UserID,
                $"Dear {user.FirstName} {user.LastName}, your loan application with ID #{application.Id} has been {application.Status} due to the following reason: {rejected.Comment}");

            await _db.SaveChangesAsync();

            // Redirect or return confirmation view
            return RedirectToAction("Index", "Admin"); // Or wherever makes sense
        }

        [HttpGet]
        public async Task<IActionResult> GetApprovedLoans()
        {
            var approvedLoan = await _db.LoanApplications
                .Include(l => l.LoanType)
                .Include(l => l.Customer)
                .Include(l => l.UploadedDocuments)
                .Where(l => l.Status == "approved")
                .ToListAsync();

            ViewBag.countApproved = approvedLoan.Count;
            return View(approvedLoan);
        }

        public async Task<IActionResult> GetRejectedLoans()
        {
            var rejectedLoan = await _db.LoanApplications
                .Include(l => l.LoanType)
                .Include(l => l.Customer)
                .Include(l => l.UploadedDocuments)
                .Where(l => l.Status.ToLower() == "rejected")
                .ToListAsync();

            ViewBag.countRejected = rejectedLoan.Count;
            return View(rejectedLoan);
        }

        public async Task<IActionResult> GetPendingLoans()
        {
            var pendingLoan = await _db.LoanApplications
                .Include(l => l.LoanType)
                .Include(l => l.Customer)
                .Include(l => l.UploadedDocuments)
                .Where(l => l.Status == "pending")
                .ToListAsync();

            ViewBag.countPending = pendingLoan.Count;
            return View(pendingLoan);
        }

        [HttpPost]
        public async Task<IActionResult> ApprovalMessage(ApprovalLog obj, int id)
        {
            if (id == 0)
                return NotFound();

            var application = await _db.LoanApplications.FindAsync(id);
            if (application == null)
                return NotFound();

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
