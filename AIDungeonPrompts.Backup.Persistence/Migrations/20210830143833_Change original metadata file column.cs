using Microsoft.EntityFrameworkCore.Migrations;

namespace AIDungeonPrompts.Backup.Persistence.Migrations
{
    public partial class Changeoriginalmetadatafilecolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OriginalFile",
                table: "Prompts",
                newName: "NovelAiScenario");

            migrationBuilder.AddColumn<string>(
                name: "HoloAiScenario",
                table: "Prompts",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoloAiScenario",
                table: "Prompts");

            migrationBuilder.RenameColumn(
                name: "NovelAiScenario",
                table: "Prompts",
                newName: "OriginalFile");
        }
    }
}
