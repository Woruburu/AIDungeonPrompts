using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Application.Helpers;
using AIDungeonPrompts.Domain.Entities;
using AIDungeonPrompts.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIDungeonPrompts.Application.Queries.GetReports
{
	public class GetReportsQuery : IRequest<List<GetReportViewModel>>
	{
		public GetReportsQuery(RoleEnum role)
		{
			Role = role;
		}

		public RoleEnum Role { get; set; }
	}

	public class GetReportsQueryHandler : IRequestHandler<GetReportsQuery, List<GetReportViewModel>>
	{
		private readonly IAIDungeonPromptsDbContext _dbContext;

		public GetReportsQueryHandler(IAIDungeonPromptsDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<GetReportViewModel>> Handle(GetReportsQuery request,
			CancellationToken cancellationToken = default)
		{
			if (!RoleHelper.CanEdit(request.Role))
			{
				throw new GetReportUnauthorizedUserException();
			}

			IQueryable<Report>? query = _dbContext.Reports.Include(e => e.Prompt).AsQueryable();
			if (request.Role == RoleEnum.TagEdit)
			{
				query = query.Where(e =>
					e.ReportReason == ReportReason.IncorrectTags ||
					e.ReportReason == ReportReason.UntaggedNsfw);
			}

			return await query
				.AsNoTracking()
				.Select(report => new GetReportViewModel
				{
					Id = report.Id,
					ExtraDetails = report.ExtraDetails,
					PromptId = report.PromptId,
					PromptTitle = report.Prompt!.Title,
					ReportReason = report.ReportReason,
					Cleared = report.Cleared
				}).ToListAsync();
		}
	}
}
