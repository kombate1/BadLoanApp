using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BadLoan.Models
{
    public class UploadedDocument
    {
        [Key]
        public int DocumentId { get; set; }

        public int LoanApplicationId { get; set; }
        //public LoanApplication LoanApplication { get; set; }

        [Required]
        public string? FilePath { get; set; }  // e.g., "/uploads/loan123/id-proof.pdf"

        //[Required]
        //public byte[] Content { get; set; }   // e.g., "id-proof.pdf"

       


    }
}
