using System.Threading.Tasks;
using AIDungeonPrompts.Application.Commands.ClearReport;
using AIDungeonPrompts.Test.Collections.Database;
using Xunit;

namespace AIDungeonPrompts.Test.Application.Commands
{
	public class ClearReportCommandHandlerTest : AbstractDatabaseFixtureTest
	{
		private readonly ClearReportCommandHandler _handler;

		public ClearReportCommandHandlerTest(DatabaseFixture fixture) : base(fixture)
		{
			_handler = new ClearReportCommandHandler(DbContext);
		}

		[Fact]
		public async Task Test()
		{
			//arrange
			var command = new ClearReportCommand();

			//act
			await _handler.Handle(command, default);

			//assert
		}
	}
}
