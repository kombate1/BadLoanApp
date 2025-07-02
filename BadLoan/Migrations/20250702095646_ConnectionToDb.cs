using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BadLoan.Migrations
{
    /// <inheritdoc />
    public partial class ConnectionToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Content",
                table: "UploadedDocuments",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "UploadedDocuments");
        }
    }
}
