using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JTMS.Migrations
{
    /// <inheritdoc />
    public partial class moldMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Molds_Projects_ProjectId",
                table: "Molds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Molds",
                table: "Molds");

            migrationBuilder.RenameTable(
                name: "Molds",
                newName: "MoldModel");

            migrationBuilder.RenameIndex(
                name: "IX_Molds_ProjectId",
                table: "MoldModel",
                newName: "IX_MoldModel_ProjectId");

            migrationBuilder.AddColumn<Guid>(
                name: "MainMoldId",
                table: "MoldModel",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MoldModel",
                table: "MoldModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MoldModel_Projects_ProjectId",
                table: "MoldModel",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MoldModel_Projects_ProjectId",
                table: "MoldModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MoldModel",
                table: "MoldModel");

            migrationBuilder.DropColumn(
                name: "MainMoldId",
                table: "MoldModel");

            migrationBuilder.RenameTable(
                name: "MoldModel",
                newName: "Molds");

            migrationBuilder.RenameIndex(
                name: "IX_MoldModel_ProjectId",
                table: "Molds",
                newName: "IX_Molds_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Molds",
                table: "Molds",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Molds_Projects_ProjectId",
                table: "Molds",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
