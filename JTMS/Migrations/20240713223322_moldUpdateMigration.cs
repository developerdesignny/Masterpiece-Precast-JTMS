using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JTMS.Migrations
{
    /// <inheritdoc />
    public partial class moldUpdateMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Process1Count",
                table: "Molds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Process2Count",
                table: "Molds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Process3Count",
                table: "Molds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Process4Count",
                table: "Molds",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Process1Count",
                table: "Molds");

            migrationBuilder.DropColumn(
                name: "Process2Count",
                table: "Molds");

            migrationBuilder.DropColumn(
                name: "Process3Count",
                table: "Molds");

            migrationBuilder.DropColumn(
                name: "Process4Count",
                table: "Molds");
        }
    }
}
