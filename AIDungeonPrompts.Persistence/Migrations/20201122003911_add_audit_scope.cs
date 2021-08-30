using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AIDungeonPrompts.Persistence.Migrations
{
    public partial class add_audit_scope : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuditDate",
                table: "AuditPrompts");

            migrationBuilder.AddColumn<Guid>(
                name: "AuditScopeId",
                table: "AuditPrompts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "AuditPrompts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuditScopeId",
                table: "AuditPrompts");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "AuditPrompts");

            migrationBuilder.AddColumn<DateTime>(
                name: "AuditDate",
                table: "AuditPrompts",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
