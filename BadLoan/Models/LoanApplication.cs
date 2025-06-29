﻿
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BadLoan.Models
{
    public class LoanApplication
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("CustomerId")]
        public int CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }

        [ForeignKey("LoanTypeId")]
        public int LoanTypeId { get; set; }
        public virtual LoanType? LoanType { get; set; }

        [Required]
        public int DocumentId { get; set; }

        [Required]
        public decimal RequestedAmount { get; set; }

        [Required]
        public int LoanTermYears { get; set; }

        [Required]
        public decimal TotalRepayable { get; set; }

        [Required]
        public string? Status { get; set; }

        [Required]
        public DateTime SubmittedDate { get; set; }

        [Required]
        public DateTime LastUpdated { get; set; }

        public virtual ICollection<ApprovalLog> ApprovalLogs { get; set; }
        public virtual ICollection<UploadedDocument> Documents { get; set; }
    }
}
