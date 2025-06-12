using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JTMS.Migrations
{
    /// <inheritdoc />
    public partial class MoldModelUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Projects_ProjectModelId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectModelId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectModelId",
                table: "Projects");

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "Molds",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Molds_ProjectId",
                table: "Molds",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Molds_Projects_ProjectId",
                table: "Molds",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Molds_Projects_ProjectId",
                table: "Molds");

            migrationBuilder.DropIndex(
                name: "IX_Molds_ProjectId",
                table: "Molds");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Molds");

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectModelId",
                table: "Projects",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectModelId",
                table: "Projects",
                column: "ProjectModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Projects_ProjectModelId",
                table: "Projects",
                column: "ProjectModelId",
                principalTable: "Projects",
                principalColumn: "Id");
        }
    }
}
