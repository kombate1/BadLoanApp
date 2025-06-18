using System;
using System.ComponentModel.DataAnnotations;

namespace BadLoan.Models
{
    public class UploadedDocument
    {
        [Key]
        public int DocumentId { get; set; }

        [Required]
        public int LoanApplicationId { get; set; }

        [Required]
        public string FileType { get; set; }  // e.g., "PDF", "JPG"

        [Required]
        public string FilePath { get; set; }  // e.g., "/uploads/loan123/id-proof.pdf"
    }
}
