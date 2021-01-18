using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIDungeonPrompts.Backup.Persistence.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Prompts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AuthorsNote = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Memory = table.Column<string>(type: "TEXT", nullable: true),
                    Nsfw = table.Column<bool>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: true),
                    PromptContent = table.Column<string>(type: "TEXT", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Quests = table.Column<string>(type: "TEXT", nullable: true),
                    Tags = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    CorrelationId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateEdited = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prompts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorldInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Entry = table.Column<string>(type: "TEXT", nullable: false),
                    Keys = table.Column<string>(type: "TEXT", nullable: false),
                    PromptId = table.Column<int>(type: "INTEGER", nullable: false),
                    CorrelationId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateEdited = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorldInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorldInfos_Prompts_PromptId",
                        column: x => x.PromptId,
                        principalTable: "Prompts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Prompts_CorrelationId",
                table: "Prompts",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_WorldInfos_CorrelationId",
                table: "WorldInfos",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_WorldInfos_PromptId",
                table: "WorldInfos",
                column: "PromptId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorldInfos");

            migrationBuilder.DropTable(
                name: "Prompts");
        }
    }
}
