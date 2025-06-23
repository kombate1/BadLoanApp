using BadLoan.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BadLoan.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<LoanType> LoanTypes { get; set; }

        public DbSet<LoanApplication> LoanApplications { get; set; }
        public DbSet<ApprovalLog> ApprovalLogs { get; set; }
        public DbSet<UploadedDocument> UploadedDocuments { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);

        //    ModelBuilder.Entity<LoanApplication>()
        //       .HasMany(e => e.ApprovalLogs)
        //       .WithOne(e => e.LoanApplication)
        //       .HasForeignKey(e => e.LoanApplicationId)
        //       .IsRequired();
        //}
    }
}
