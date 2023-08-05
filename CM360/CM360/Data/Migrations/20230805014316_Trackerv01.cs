using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CM360.Data.Migrations
{
    /// <inheritdoc />
    public partial class Trackerv01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "Zip",
                table: "Contact");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Contact",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeNumber",
                table: "Contact",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExpenseType",
                table: "Contact",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "EmployeeNumber",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "ExpenseType",
                table: "Contact");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Contact",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Contact",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Contact",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Zip",
                table: "Contact",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
