﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BadLoan.Migrations
{
    /// <inheritdoc />
    public partial class addNewPropertiesToNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Notifications");
        }
    }
}
