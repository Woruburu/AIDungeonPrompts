using System.Threading.Tasks;
using AIDungeonPrompts.Application.Queries.GetReports;
using AIDungeonPrompts.Domain.Entities;
using AIDungeonPrompts.Domain.Enums;
using AIDungeonPrompts.Test.Collections.Database;
using Xunit;

namespace AIDungeonPrompts.Test.Application.Queries.GetReports
{
	public class GetReportsQueryHandlerTest : AbstractDatabaseFixtureTest
	{
		private readonly GetReportsQueryHandler _handler;

		public GetReportsQueryHandlerTest(DatabaseFixture fixture) : base(fixture)
		{
			_handler = new GetReportsQueryHandler(DbContext);
		}

		[Fact]
		public async Task Handle_ReturnsAllReports_WhenUserHasFeildEditRole()
		{
			//arrange
			var prompt = new Prompt();
			DbContext.Reports.AddRange(
				new Report { ReportReason = ReportReason.Duplicate, Prompt = prompt },
				new Report { ReportReason = ReportReason.IllegalContent, Prompt = prompt },
				new Report { ReportReason = ReportReason.IncorrectTags, Prompt = prompt },
				new Report { ReportReason = ReportReason.LowQuality, Prompt = prompt },
				new Report { ReportReason = ReportReason.NeedsCorrection, Prompt = prompt },
				new Report { ReportReason = ReportReason.Other, Prompt = prompt },
				new Report { ReportReason = ReportReason.UntaggedNsfw, Prompt = prompt });
			await DbContext.SaveChangesAsync();
			var query = new GetReportsQuery(RoleEnum.FieldEdit);

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Equal(7, actual.Count);
		}

		[Fact]
		public async Task Handle_ReturnsAllTagEditableRelatedReports_WhenUserHasTagEditRole()
		{
			//arrange
			var prompt = new Prompt();
			DbContext.Reports.AddRange(
				new Report { ReportReason = ReportReason.Duplicate, Prompt = prompt },
				new Report { ReportReason = ReportReason.IllegalContent, Prompt = prompt },
				new Report { ReportReason = ReportReason.IncorrectTags, Prompt = prompt },
				new Report { ReportReason = ReportReason.LowQuality, Prompt = prompt },
				new Report { ReportReason = ReportReason.NeedsCorrection, Prompt = prompt },
				new Report { ReportReason = ReportReason.Other, Prompt = prompt },
				new Report { ReportReason = ReportReason.UntaggedNsfw, Prompt = prompt });
			await DbContext.SaveChangesAsync();
			var query = new GetReportsQuery(RoleEnum.TagEdit);

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Equal(2, actual.Count);
		}

		[Theory]
		[InlineData(RoleEnum.TagEdit)]
		[InlineData(RoleEnum.FieldEdit)]
		[InlineData(RoleEnum.Delete)]
		public async Task Handle_ReturnsNoReports_WhenDatabaseIsEmpty(RoleEnum role)
		{
			//arrange
			var query = new GetReportsQuery(role);

			//act
			var actual = await _handler.Handle(query);

			//assert
			Assert.Empty(actual);
		}

		[Fact]
		public async Task Handle_ThrowsUnauthorizedUserReportException_WhenThereIsNoUserLoggedIn()
		{
			//arrange
			var query = new GetReportsQuery(RoleEnum.None);

			//act + assert
			await Assert.ThrowsAsync<GetReportUnauthorizedUserException>(async () =>
				await _handler.Handle(query)
			);
		}
	}
}
