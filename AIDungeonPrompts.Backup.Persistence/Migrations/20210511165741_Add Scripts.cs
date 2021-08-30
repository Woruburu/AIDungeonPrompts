using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIDungeonPrompts.Backup.Persistence.Migrations
{
    public partial class AddScripts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ScriptZip",
                table: "Prompts",
                type: "BLOB",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScriptZip",
                table: "Prompts");
        }
    }
}
