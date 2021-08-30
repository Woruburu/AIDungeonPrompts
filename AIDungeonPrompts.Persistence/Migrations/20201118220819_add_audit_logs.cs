using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace AIDungeonPrompts.Persistence.Migrations
{
	public partial class add_audit_logs : Migration
	{
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "AuditPrompts");
		}

		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "AuditPrompts",
				columns: table => new
				{
					Id = table.Column<int>(type: "integer", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					AuditDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
					Entry = table.Column<string>(type: "jsonb", nullable: false),
					PromptId = table.Column<int>(type: "integer", nullable: false),
					DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
					DateEdited = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AuditPrompts", x => x.Id);
				});
		}
	}
}
