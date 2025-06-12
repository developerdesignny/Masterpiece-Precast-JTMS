using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JTMS.Migrations
{
    /// <inheritdoc />
    public partial class submoldUpdateMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PalletID",
                table: "Molds");

            migrationBuilder.AddColumn<string>(
                name: "PalletID",
                table: "SubMolds",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PalletID",
                table: "SubMolds");

            migrationBuilder.AddColumn<string>(
                name: "PalletID",
                table: "Molds",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
