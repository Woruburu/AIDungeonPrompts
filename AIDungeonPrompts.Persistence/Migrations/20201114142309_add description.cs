using Microsoft.EntityFrameworkCore.Migrations;

namespace AIDungeonPrompts.Persistence.Migrations
{
    public partial class adddescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Prompts",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Prompts");
        }
    }
}
