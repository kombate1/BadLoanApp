using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BadLoan.Migrations
{
    /// <inheritdoc />
    public partial class FileUploadChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UploadedDocuments_LoanApplications_LoanApplicationId",
                table: "UploadedDocuments");

            migrationBuilder.DropIndex(
                name: "IX_UploadedDocuments_LoanApplicationId",
                table: "UploadedDocuments");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "UploadedDocuments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Content",
                table: "UploadedDocuments",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

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
    }
}
