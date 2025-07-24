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
        private readonly UserManager<IdentityUser>
    _userManager;
        private readonly IConfiguration _configuration;
        private readonly EligibilityService _eligibilityService;
        private static readonly string[] AllowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };



        public LoanApplicationController(
        ApplicationDbContext db,
        UserManager<IdentityUser>
            userManager,
            IConfiguration configuration,
            EligibilityService eligibilityService
            )
        {
            _db = db;
            _userManager = userManager;
            _configuration = configuration;
            _eligibilityService = eligibilityService;

        }




        [HttpGet]
        public async Task<IActionResult>
            Index()
        {
            if (User?.Identity == null || !User.Identity.IsAuthenticated || string.IsNullOrEmpty(User.Identity.Name))
            {
                TempData["ErrorMessage"] = "Please login to access Loan Application Or SignUp if you don't have an account";
                return RedirectToPage("/Account/Login", new { area = "Identity", returnUrl = Url.Action("Index", "LoanApplication") }); // Redirect to login page and return back to this page after login
            }
            var user = await _userManager.FindByNameAsync(User!.Identity!.Name!);

            var customer = await _db.Customers.Where(c => c.UserId == user!.Id).FirstOrDefaultAsync();

            if (customer == null || customer.CustomerId == 0)
            {
                TempData["ErrorMessage"] = "Customer record not found because Customer details have not been filled. PLease fill in the details below.";
                return RedirectToAction("Create", "Home", new { returnUrl = Url.Action("Index", "LoanApplication") });
            }



            LoanAttachmentViewModel viewModel = new LoanAttachmentViewModel()
            {
                LoanApplicationDetails = new LoanApplication(),
                CustomerId = customer!.CustomerId,
                Occupation = customer!.Occupation,
                AnnualIncome = customer!.AnnualIncome,
                UploadedDocuments = new List<UploadedDocument>()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult>
            Index(LoanAttachmentViewModel obj, IFormCollection myFiles)
        {

            var user = await _userManager.FindByNameAsync(User!.Identity!.Name!);

            var customer = await _db.Customers.Where(c => c.UserId == user!.Id).FirstOrDefaultAsync();

            //obj.LoanApplicationDetails.AnnualIncome = customer.AnnualIncome;


            //var validLoanType = await _db.LoanTypes.FirstOrDefaultAsync();

            if (obj.LoanType == null)
            {
                ViewBag.MessageHtml = "Please select a loan type.";
                return View(obj);
            }
            
                var loantypeInput = obj.LoanType;
                var validLoanType = await _db.LoanTypes.FirstOrDefaultAsync(l => l.LoanTypeName == loantypeInput);
            

               
            
             



            obj.LoanApplicationDetails.LoanTypeId = validLoanType.LoanTypeId;
            string loantype = validLoanType.LoanTypeName;
            int duration = obj.LoanApplicationDetails.Duration;
            obj.LoanApplicationDetails.AnnualIncome = customer.AnnualIncome;

            decimal annualIncome = obj.LoanApplicationDetails.AnnualIncome;
            decimal loanAmount = obj.LoanApplicationDetails.LoanAmount;


            var results = _eligibilityService.LoanEligibility(annualIncome, duration, loantype, loanAmount);

            ViewBag.MessageHtml = results.Message;


            //var loanTypeId = Convert.ToInt32(obj.LoanType);
            //var findLoanType = _db.LoanTypes.Where(c => c.LoanTypeId == loanTypeId).FirstOrDefault().LoanTypeName;
            //obj.Calculation!.LoanType = findLoanType;

            var proofOfIncomeAttachment = await HandleFileAttachments(myFiles, "proofOfIncome");
            var employmentAttachment = await HandleFileAttachments(myFiles, "employmentAttachments");

            if (!proofOfIncomeAttachment.IsValid || !employmentAttachment.IsValid)
            {
                var errorMessages = new List<string>();
                if (!proofOfIncomeAttachment.IsValid)
                    errorMessages.Add(proofOfIncomeAttachment.ErrorMessage);
                if (!employmentAttachment.IsValid)
                    errorMessages.Add(employmentAttachment.ErrorMessage);

                ViewBag.MessageHtml = string.Join("<br/>", errorMessages);
                return View(obj);
            }



            bool isEligible = results.IsEligible;

            if (obj?.LoanApplicationDetails == null)
            {
                ViewBag.MessageHtml = "Loan application details are missing.";
                return View(obj);
            }


            obj.LoanApplicationDetails.LoanTypeId = validLoanType.LoanTypeId;
            obj.LoanApplicationDetails.CustomerId = customer.CustomerId;
            if (obj.LoanApplicationDetails != null && isEligible)
            {
                _db.LoanApplications.Add(obj!.LoanApplicationDetails!);
                await _db.SaveChangesAsync();
            }
            else
            {
                ViewBag.MessageHtml = results.Message;
                return View(obj);
            }

           


            if (!string.IsNullOrEmpty(proofOfIncomeAttachment.FilePath) && isEligible)
            {
                UploadedDocument uploadedDocument = new UploadedDocument
                {
                    FilePath = proofOfIncomeAttachment.FilePath.ToString(),
                    FileType = "Proof Of Income Attachment",
                    LoanApplicationId = obj.LoanApplicationDetails!.Id
                };

                _db.UploadedDocuments.Add(uploadedDocument);
            }





            if (!string.IsNullOrEmpty(employmentAttachment.FilePath) && isEligible)
            {
                UploadedDocument uploadedDoc = new UploadedDocument
                {
                    FilePath = employmentAttachment.FilePath.ToString(),
                    FileType = "Employment Attachment",
                    LoanApplicationId = obj.LoanApplicationDetails!.Id
                };

                _db.UploadedDocuments.Add(uploadedDoc);
            }


            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Loan application submitted successfully!";
            return RedirectToAction("Index"); // or redirect to a success page


        }




        private async Task<(bool IsValid, string FilePath, string ErrorMessage)>
            HandleFileAttachments(IFormCollection collectedFiles, string fileKey)
        {
            //var drivePath = _configuration["PathDetails:DrivePath"];
            var drivePath = _configuration["PathDetails:DrivePath"];
            if (string.IsNullOrWhiteSpace(drivePath))
                throw new InvalidOperationException("Drive path configuration is missing.");

            var attachments = collectedFiles.Files
            .Where(f => f.Name == fileKey && f.Length > 0)
            .ToList();

            if (!attachments.Any())
                return (true, string.Empty, string.Empty);


            //Validate file extensions
            foreach (var file in attachments)
            {
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!AllowedExtensions.Contains(fileExtension))
                {
                    return (false, string.Empty, "Only PDF and image files (.pdf, .jpg, .jpeg, .png) are allowed.");
                }
            }


            //var invalidFiles = new List<string>();  // creates a list that adds all invalid files so that the error displays after checking and sees one has an invalid 
            //foreach (var file in attachments)
            //{
            //    var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            //    if (!AllowedExtensions.Contains(fileExtension))
            //    {
            //        invalidFiles.Add(file.FileName);
            //    }
            //}
            //if (invalidFiles.Any())
            //{
            //    var errorMsg = "Only PDF and image files (.pdf, .jpg, .jpeg, .png) are allowed. Invalid file(s): " +
            //                   string.Join(", ", invalidFiles);
            //    return (false, string.Empty, errorMsg);
            //}

            // Validate files before processing
            //var validationResult = _fileValidationService.ValidateFiles(attachments);
            //if (!validationResult.IsValid)
            //{
            //    _errorLogs.LogExceptions(new Exception($"File validation failed: {validationResult.ErrorMessage}"), "FileValidation");
            //    throw new InvalidOperationException(validationResult.ErrorMessage);
            //}

            //var uploadFolder = Path.Combine(drivePath, "Uploaded Documents");
            var uploadFolder = Path.Combine(drivePath, "Uploaded Documents");
            Directory.CreateDirectory(uploadFolder); // Ensure path exists

            var fileUrls = new List<string>
                ();

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
                fileUrls.Add($"{relativeUrl}");
            }

            return (true, string.Join("", fileUrls), string.Empty);
        }

        [HttpGet]
        public async Task<IActionResult>
            Details()
        {

            var applications = await _db.LoanApplications.
            Include(l => l.LoanType).
            Include(l => l.Customer).
            Include(l => l.UploadedDocuments).
            ToListAsync();


            return View(applications);
        }

        public async Task<IActionResult>
            DownloadFile(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return NotFound("File path not provided");
                }

                filePath = filePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
                var prefix = "Document Uploads" + Path.DirectorySeparatorChar;
                if (filePath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    filePath = filePath.Substring(prefix.Length);
                }


                // Get the drive path from configuration
                // var drivePath = _configuration["NEWCONFIG:drivePath"];
                var drivePath = _configuration["PathDetails:DrivePath"];
                if (string.IsNullOrEmpty(drivePath))
                {
                    return StatusCode(500, "Drive path configuration is missing");
                }

                // Handle both formats of file paths
                string fullPath;
                if (filePath.StartsWith("Uploaded Documents"))
                {
                    // Handle relative path format
                    fullPath = Path.Combine(drivePath, filePath);
                }
                else
                {
                    //C:\\YebesiSavingsLoans / Document Uploads / internet_speed_78abd740 - a0b7 - 44f5 - bf4b - b8080f19542a.pdf
                    //"C:\YebesiSavingsLoans\Uploaded Documents\internet_speed_78abd740-a0b7-44f5-bf4b-b8080f19542a.pdf"

                    // Handle full path format
                    //fullPath = Path.Combine(Directory.GetCurrentDirectory(), drivePath, "uploads", "receipts", filePath);
                    fullPath = Path.Combine(drivePath, "Uploaded Documents", filePath);

                }

                if (!System.IO.File.Exists(fullPath))
                {
                    return NotFound("File not found");
                }

                // Get the file name from the path
                var fileName = Path.GetFileName(fullPath);

                // Read the file
                var memory = new MemoryStream();
                using (var stream = new FileStream(fullPath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                Response.Headers["Content-Disposition"] = $"inline; filename={fileName}";

                // Return the file
                return File(memory, GetContentType(fullPath), fileName);


            }
            catch (Exception ex)
            {

                return StatusCode(500, "Error downloading file");
            }


        }

        private string GetContentType(string path)
        {
            var extension = Path.GetExtension(path).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };
        }


    }
}
