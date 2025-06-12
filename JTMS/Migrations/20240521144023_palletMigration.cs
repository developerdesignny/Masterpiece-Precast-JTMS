using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JTMS.Migrations
{
    /// <inheritdoc />
    public partial class palletMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pallet_Projects_ProjectId",
                table: "Pallet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pallet",
                table: "Pallet");

            migrationBuilder.RenameTable(
                name: "Pallet",
                newName: "Pallets");

            migrationBuilder.RenameColumn(
                name: "PalletGroup",
                table: "Molds",
                newName: "PalletID");

            migrationBuilder.RenameIndex(
                name: "IX_Pallet_ProjectId",
                table: "Pallets",
                newName: "IX_Pallets_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pallets",
                table: "Pallets",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Pallets_Projects_ProjectId",
                table: "Pallets",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pallets_Projects_ProjectId",
                table: "Pallets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pallets",
                table: "Pallets");

            migrationBuilder.RenameTable(
                name: "Pallets",
                newName: "Pallet");

            migrationBuilder.RenameColumn(
                name: "PalletID",
                table: "Molds",
                newName: "PalletGroup");

            migrationBuilder.RenameIndex(
                name: "IX_Pallets_ProjectId",
                table: "Pallet",
                newName: "IX_Pallet_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pallet",
                table: "Pallet",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Pallet_Projects_ProjectId",
                table: "Pallet",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
