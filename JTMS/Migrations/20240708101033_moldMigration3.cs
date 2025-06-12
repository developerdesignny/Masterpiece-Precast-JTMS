using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JTMS.Migrations
{
    /// <inheritdoc />
    public partial class moldMigration3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Process1Complete",
                table: "Molds");

            migrationBuilder.DropColumn(
                name: "Process2Complete",
                table: "Molds");

            migrationBuilder.DropColumn(
                name: "Process3Complete",
                table: "Molds");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Process1Complete",
                table: "Molds",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Process2Complete",
                table: "Molds",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Process3Complete",
                table: "Molds",
                type: "tinyint(1)",
                nullable: true);
        }
    }
}
