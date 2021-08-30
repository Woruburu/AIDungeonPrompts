using Microsoft.EntityFrameworkCore.Migrations;

namespace AIDungeonPrompts.Persistence.Migrations
{
    public partial class add_holo_ai : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HoloAiScenario",
                table: "Prompts",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoloAiScenario",
                table: "Prompts");
        }
    }
}
