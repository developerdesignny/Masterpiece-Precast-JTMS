using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JTMS.Migrations
{
    /// <inheritdoc />
    public partial class projectNotesMode2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectNotes_Molds_MoldId",
                table: "ProjectNotes");

            migrationBuilder.RenameColumn(
                name: "MoldId",
                table: "ProjectNotes",
                newName: "ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectNotes_MoldId",
                table: "ProjectNotes",
                newName: "IX_ProjectNotes_ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectNotes_Projects_ProjectId",
                table: "ProjectNotes",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectNotes_Projects_ProjectId",
                table: "ProjectNotes");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "ProjectNotes",
                newName: "MoldId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectNotes_ProjectId",
                table: "ProjectNotes",
                newName: "IX_ProjectNotes_MoldId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectNotes_Molds_MoldId",
                table: "ProjectNotes",
                column: "MoldId",
                principalTable: "Molds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
