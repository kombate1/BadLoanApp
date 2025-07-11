using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BadLoan.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreDetailsToLoanApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "LoanApplications");

            migrationBuilder.RenameColumn(
                name: "YearlyIncome",
                table: "Customers",
                newName: "AnnualIncome");

            migrationBuilder.AddColumn<decimal>(
                name: "AnnualIncome",
                table: "LoanApplications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "LoanApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "LoanAmount",
                table: "LoanApplications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_UploadedDocuments_LoanApplicationId",
                table: "UploadedDocuments",
                column: "LoanApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_UploadedDocuments_LoanApplications_LoanApplicationId",
                table: "UploadedDocuments",
                column: "LoanApplicationId",
                principalTable: "LoanApplications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UploadedDocuments_LoanApplications_LoanApplicationId",
                table: "UploadedDocuments");

            migrationBuilder.DropIndex(
                name: "IX_UploadedDocuments_LoanApplicationId",
                table: "UploadedDocuments");

            migrationBuilder.DropColumn(
                name: "AnnualIncome",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "LoanAmount",
                table: "LoanApplications");

            migrationBuilder.RenameColumn(
                name: "AnnualIncome",
                table: "Customers",
                newName: "YearlyIncome");

            migrationBuilder.AddColumn<int>(
                name: "DocumentId",
                table: "LoanApplications",
                type: "int",
                nullable: true);
        }
    }
}
