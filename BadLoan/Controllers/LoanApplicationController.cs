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
        private readonly IConfiguration _configuration;


        public LoanApplicationController(
            ApplicationDbContext db,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration
            )
        {
            _db = db;
            _userManager = userManager;
            _configuration = configuration;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User!.Identity!.Name!);

            var customer = await _db.Customers.Where(c => c.UserId == user!.Id).FirstOrDefaultAsync();

            LoanAttachmentViewModel viewModel = new LoanAttachmentViewModel()
            {
                LoanApplicationDetails = new LoanApplication(),
                CustomerId = customer!.CustomerId,
                Occupation = customer!.Occupation,
                YearlyIncome = customer!.YearlyIncome,
                UploadedDocuments = new List<UploadedDocument>()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoanAttachmentViewModel obj, IFormCollection myFiles)
        {

            var user = await _userManager.FindByNameAsync(User!.Identity!.Name!);

            var customer = await _db.Customers.Where(c => c.UserId == user!.Id).FirstOrDefaultAsync();


            var validLoanType = await _db.LoanTypes.FirstOrDefaultAsync();
            if (validLoanType == null)
            {
                // Optional: create a dummy loan type if your table is empty (for test only)
                validLoanType = new LoanType { LoanTypeName = "Test Loan" };
                _db.LoanTypes.Add(validLoanType);
                await _db.SaveChangesAsync();

            }

            obj.LoanApplicationDetails.LoanTypeId = validLoanType.LoanTypeId;
            obj.LoanApplicationDetails.CustomerId = customer.CustomerId;
            if (obj.LoanApplicationDetails != null)
            {
                _db.LoanApplications.Add(obj!.LoanApplicationDetails!);
                await _db.SaveChangesAsync();
            }

            var proofOfIncomeAttachment = await HandleFileAttachments(myFiles, "proofOfIncome");
            var employmentAttachment = await HandleFileAttachments(myFiles, "employmentAttachments");



            if (proofOfIncomeAttachment != null)
            {
                UploadedDocument uploadedDocument = new UploadedDocument
                {
                    FilePath = proofOfIncomeAttachment.ToString(),
                    LoanApplicationId = obj.LoanApplicationDetails!.Id
                };

                 _db.UploadedDocuments.Add(uploadedDocument);
            }





            if (employmentAttachment != null)
            {
                UploadedDocument uploadedDoc = new UploadedDocument
                {
                    FilePath = employmentAttachment.ToString(),
                    LoanApplicationId = obj.LoanApplicationDetails!.Id
                };

                _db.UploadedDocuments.Add(uploadedDoc);
            }

            await _db.SaveChangesAsync();

            return View(obj);

        }

           
        

        private async Task<string> HandleFileAttachments(IFormCollection collectedFiles, string fileKey)
        {
            //var drivePath = _configuration["PathDetails:DrivePath"];
            var drivePath = _configuration["PathDetails:DrivePath"];
            if (string.IsNullOrWhiteSpace(drivePath))
                throw new InvalidOperationException("Drive path configuration is missing.");

            var attachments = collectedFiles.Files
                .Where(f => f.Name == fileKey && f.Length > 0)
                .ToList();

            if (!attachments.Any())
                return string.Empty;

            // Validate files before processing
            //var validationResult = _fileValidationService.ValidateFiles(attachments);
            //if (!validationResult.IsValid)
            //{
            //    _errorLogs.LogExceptions(new Exception($"File validation failed: {validationResult.ErrorMessage}"), "FileValidation");
            //    throw new InvalidOperationException(validationResult.ErrorMessage);
            //}

            var uploadFolder = Path.Combine(drivePath, "Uploaded Documents");
            Directory.CreateDirectory(uploadFolder); // Ensure path exists

            var fileUrls = new List<string>();

            foreach (var file in attachments)
            {
                var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                //var safeFileName = SanitizeFileName(originalFileName);
                var fileExtension = Path.GetExtension(file.FileName);
                //var uniqueFileName = $"{safeFileName}_{Guid.NewGuid()}{fileExtension}";
                var uniqueFileName = $"{originalFileName}_{Guid.NewGuid()}{fileExtension}";

                var filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Relative URL for later usage
                var relativeUrl = Path.Combine("Document Uploads", uniqueFileName)
                                  .Replace("\\", "/");
                fileUrls.Add($"##{relativeUrl}");
            }

            return string.Join("", fileUrls);
        }

    }
}
