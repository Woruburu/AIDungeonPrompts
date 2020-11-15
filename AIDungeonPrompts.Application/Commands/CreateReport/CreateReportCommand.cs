using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Domain.Entities;
using AIDungeonPrompts.Domain.Enums;
using MediatR;

namespace AIDungeonPrompts.Application.Commands.CreateReport
{
	public class CreateReportCommand : IRequest
	{
		[Display(Name = "Additional Details")]
		public string? ExtraDetails { get; set; }
		public int PromptId { get; set; }
		[Display(Name = "Report Reason")]
		public ReportReason ReportReason { get; set; }
	}

	public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand>
	{
		private readonly IAIDungeonPromptsDbContext _dbContext;

		public CreateReportCommandHandler(IAIDungeonPromptsDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<Unit> Handle(CreateReportCommand request, CancellationToken cancellationToken)
		{
			_dbContext.Reports.Add(new Report
			{
				DateCreated = DateTime.Now,
				ExtraDetails = request.ExtraDetails,
				PromptId = request.PromptId,
				ReportReason = request.ReportReason
			});
			await _dbContext.SaveChangesAsync(cancellationToken);
			return Unit.Value;
		}
	}
}
