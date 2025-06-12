using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JTMS.Migrations
{
    /// <inheritdoc />
    public partial class palletModel2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pallet_Molds_MoldId",
                table: "Pallet");

            migrationBuilder.RenameColumn(
                name: "MoldId",
                table: "Pallet",
                newName: "ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Pallet_MoldId",
                table: "Pallet",
                newName: "IX_Pallet_ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pallet_Projects_ProjectId",
                table: "Pallet",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pallet_Projects_ProjectId",
                table: "Pallet");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "Pallet",
                newName: "MoldId");

            migrationBuilder.RenameIndex(
                name: "IX_Pallet_ProjectId",
                table: "Pallet",
                newName: "IX_Pallet_MoldId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pallet_Molds_MoldId",
                table: "Pallet",
                column: "MoldId",
                principalTable: "Molds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
