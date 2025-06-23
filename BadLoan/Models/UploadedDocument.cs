using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BadLoan.Models
{
    public class UploadedDocument
    {
        [Key]
        public int DocumentId { get; set; }

        
        [ForeignKey("LoanApplicationId")]
        public int LoanApplicationId { get; set; }
        public LoanApplication? LoanApplication { get; set; }

        [Required]
        public string? FileType { get; set; }  // e.g., "Ghana card", "proof of payment", "employment offer letter", etc.

        [Required]
        public string? FilePath { get; set; }  // e.g., "/uploads/loan123/id-proof.pdf"

        
    }
}
