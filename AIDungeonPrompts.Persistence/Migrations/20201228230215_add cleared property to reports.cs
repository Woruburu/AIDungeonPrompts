using Microsoft.EntityFrameworkCore.Migrations;

namespace AIDungeonPrompts.Persistence.Migrations
{
    public partial class addclearedpropertytoreports : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Cleared",
                table: "Reports",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cleared",
                table: "Reports");
        }
    }
}
