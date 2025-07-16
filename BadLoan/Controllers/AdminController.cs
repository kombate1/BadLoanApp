using BadLoan.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BadLoan.Controllers
{
    public class AdminController : Controller
    {

        private readonly ApplicationDbContext _db;

        public AdminController(ApplicationDbContext db)
        {
            _db = db;
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


            return View();
        }


        public async Task<IActionResult> GetAllLoans()
        {

            var getLoans = await _db.LoanApplications.
                Include(l => l.LoanType).
                Include(l => l.Customer).
                Include(l => l.UploadedDocuments).
                ToListAsync();

            int countAllLoan = getLoans.Count;
            ViewBag.countAllLoans = countAllLoan;

            return View(getLoans);
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
                Where(l => l.Status == "rejected").
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

    }
}
