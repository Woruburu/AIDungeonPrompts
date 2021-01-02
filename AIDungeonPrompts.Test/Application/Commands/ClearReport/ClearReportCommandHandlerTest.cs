using System.Threading.Tasks;
using AIDungeonPrompts.Application.Commands.ClearReport;
using AIDungeonPrompts.Domain.Entities;
using AIDungeonPrompts.Test.Collections.Database;
using Xunit;

namespace AIDungeonPrompts.Test.Application.Commands.ClearReport
{
	public class ClearReportCommandHandlerTest : DatabaseFixtureTest
	{
		private readonly ClearReportCommandHandler _handler;

		public ClearReportCommandHandlerTest(DatabaseFixture fixture) : base(fixture)
		{
			_handler = new ClearReportCommandHandler(DbContext);
		}

		[Theory]
		[InlineData(1)]
		[InlineData(10)]
		[InlineData(64)]
		[InlineData(256)]
		public async Task Handle_SetsReportToCleared_WhenTheReportIdIsFound(int extraReports)
		{
			//arrange
			var report = new Report { Prompt = new Prompt() };
			DbContext.Reports.Add(report);
			for (var i = 0; i < extraReports; i++)
			{
				DbContext.Reports.Add(new Report { Prompt = new Prompt() });
			}
			await DbContext.SaveChangesAsync();
			var command = new ClearReportCommand(report.Id);

			//act
			await _handler.Handle(command);

			//assert
			Assert.True(DbContext.Reports.Find(report.Id).Cleared);
		}

		[Theory]
		[InlineData(-10)]
		[InlineData(0)]
		[InlineData(64)]
		[InlineData(256)]
		[InlineData(int.MaxValue)]
		public async Task Handle_ThrowsClearReportNotFoundException_WhenTheDatabaseIsEmpty(int id)
		{
			//arrange
			var command = new ClearReportCommand(id);

			//act + assert
			await Assert.ThrowsAsync<ClearReportNotFoundException>(async () => await _handler.Handle(command));
		}

		[Theory]
		[InlineData(-10)]
		[InlineData(0)]
		[InlineData(int.MaxValue)]
		public async Task Handle_ThrowsClearReportNotFoundException_WhenTheReportIdIsNotFound(int id)
		{
			//arrange
			DbContext.Reports.Add(new Report { Prompt = new Prompt() });
			await DbContext.SaveChangesAsync();
			var command = new ClearReportCommand(id);

			//act + assert
			await Assert.ThrowsAsync<ClearReportNotFoundException>(async () => await _handler.Handle(command));
		}
	}
}
