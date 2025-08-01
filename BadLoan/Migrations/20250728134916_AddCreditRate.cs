using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BadLoan.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "creditRate",
                table: "LoanApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "creditRate",
                table: "LoanApplications");
        }
    }
}
