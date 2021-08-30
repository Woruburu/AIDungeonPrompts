using Microsoft.EntityFrameworkCore.Migrations;

namespace AIDungeonPrompts.Persistence.Migrations
{
	public partial class addlogtomigrations : Migration
	{
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			//migrationBuilder.DropTable(
			//    name: "ApplicationLogs");
		}

		protected override void Up(MigrationBuilder migrationBuilder)
		{
			//migrationBuilder.CreateTable(
			//    name: "ApplicationLogs",
			//    columns: table => new
			//    {
			//        Id = table.Column<int>(type: "integer", nullable: false)
			//            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
			//        Exception = table.Column<string>(type: "text", nullable: true),
			//        Level = table.Column<string>(type: "text", nullable: false),
			//        LogEvent = table.Column<string>(type: "jsonb", nullable: false),
			//        Message = table.Column<string>(type: "text", nullable: false),
			//        Properties = table.Column<string>(type: "jsonb", nullable: false),
			//        RenderedMessage = table.Column<string>(type: "text", nullable: false),
			//        TimeStamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
			//    },
			//    constraints: table =>
			//    {
			//        table.PrimaryKey("PK_ApplicationLogs", x => x.Id);
			//    });
		}
	}
}
