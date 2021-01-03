using Microsoft.EntityFrameworkCore.Migrations;

namespace AIDungeonPrompts.Persistence.Migrations
{
    public partial class adddraftandselfreferencingtabletoprompts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDraft",
                table: "Prompts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Prompts",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prompts_ParentId",
                table: "Prompts",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prompts_Prompts_ParentId",
                table: "Prompts",
                column: "ParentId",
                principalTable: "Prompts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prompts_Prompts_ParentId",
                table: "Prompts");

            migrationBuilder.DropIndex(
                name: "IX_Prompts_ParentId",
                table: "Prompts");

            migrationBuilder.DropColumn(
                name: "IsDraft",
                table: "Prompts");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Prompts");
        }
    }
}
