using Microsoft.EntityFrameworkCore.Migrations;

namespace AIDungeonPrompts.Persistence.Migrations
{
	public partial class addnondraftview : Migration
	{
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql("DROP VIEW \"NonDraftPrompts\"");
		}

		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"
CREATE VIEW ""NonDraftPrompts"" AS
WITH RECURSIVE cte AS (
	SELECT p.""Id""
		FROM   ""Prompts"" p
		WHERE  ""ParentId"" is null and ""IsDraft"" = false
		UNION ALL
		SELECT p2.""Id""
		FROM ""Prompts"" p2
		INNER JOIN cte
		ON p2.""ParentId"" = cte.""Id""
		WHERE  p2.""Id"" <> p2.""ParentId""
)
SELECT cte.""Id""
FROM cte
;
			");
		}
	}
}
