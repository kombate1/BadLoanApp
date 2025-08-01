﻿
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http.Features;

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
        public decimal RequestedAmount { get; set; }

        [Required]
        public int LoanTermYears { get; set; }

        
        public decimal? TotalRepayable { get; set; }

        [Required]
        public string? Status { get; set; } = "Pending";

        [Required]
        public DateTime SubmittedDate { get; set; } = DateTime.Now;

        
        public DateTime? LastUpdated { get; set; }

        [Range(1, 10000000000)]
        public decimal AnnualIncome { get; set; }

        [Range(1, 31)]
        public int Duration { get; set; }

        [Range(1, 10000000000)]
        public decimal LoanAmount { get; set; }

        public string creditRate { get; set; }




        //public virtual ICollection<ApprovalLog> ApprovalLogs { get; set; }
        public virtual ICollection<UploadedDocument> UploadedDocuments { get; set; } = new List<UploadedDocument>();
    }
}
