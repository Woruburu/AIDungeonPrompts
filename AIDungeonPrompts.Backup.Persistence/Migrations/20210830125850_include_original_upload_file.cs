using Microsoft.EntityFrameworkCore.Migrations;

namespace AIDungeonPrompts.Backup.Persistence.Migrations
{
    public partial class include_original_upload_file : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginalFile",
                table: "Prompts",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalFile",
                table: "Prompts");
        }
    }
}
