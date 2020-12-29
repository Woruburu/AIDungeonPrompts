using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Application.Abstractions.Identity;
using AIDungeonPrompts.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIDungeonPrompts.Application.Queries.GetReports
{
	public class GetReportsQuery : IRequest<GetReportsViewModel>
	{
	}

	public class GetReportsQueryHandler : IRequestHandler<GetReportsQuery, GetReportsViewModel>
	{
		private readonly ICurrentUserService _currentUserService;
		private readonly IAIDungeonPromptsDbContext _dbContext;

		public GetReportsQueryHandler(IAIDungeonPromptsDbContext dbContext, ICurrentUserService currentUserService)
		{
			_dbContext = dbContext;
			_currentUserService = currentUserService;
		}

		public async Task<GetReportsViewModel> Handle(GetReportsQuery request, CancellationToken cancellationToken)
		{
			if (!_currentUserService.TryGetCurrentUser(out var user))
			{
				throw new UnauthorizedUserReportException();
			}

			var query = _dbContext.Reports.Include(e => e.Prompt).AsQueryable();
			if (user!.Role == RoleEnum.TagEdit)
			{
				query = query.Where(e => e.ReportReason == ReportReason.IncorrectTags);
			}
			var result = await query.ToListAsync();

			return new GetReportsViewModel
			{
				Reports = result.Select(report => new GetReportViewModel
				{
					Id = report.Id,
					ExtraDetails = report.ExtraDetails,
					PromptId = report.PromptId,
					PromptTitle = report.Prompt!.Title,
					ReportReason = report.ReportReason,
					Cleared = report.Cleared
				}).ToList()
			};
		}
	}
}
