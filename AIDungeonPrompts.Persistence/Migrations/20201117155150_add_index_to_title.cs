using Microsoft.EntityFrameworkCore.Migrations;

namespace AIDungeonPrompts.Persistence.Migrations
{
    public partial class add_index_to_title : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Prompts_Title",
                table: "Prompts",
                column: "Title");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Prompts_Title",
                table: "Prompts");
        }
    }
}
