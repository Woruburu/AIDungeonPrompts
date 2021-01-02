using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIDungeonPrompts.Application.Commands.ClearReport
{
	public class ClearReportCommand : IRequest
	{
		public ClearReportCommand(int id)
		{
			Id = id;
		}

		public int Id { get; set; }
	}

	public class ClearReportCommandHandler : IRequestHandler<ClearReportCommand>
	{
		private readonly IAIDungeonPromptsDbContext _dbContext;

		public ClearReportCommandHandler(IAIDungeonPromptsDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<Unit> Handle(ClearReportCommand request, CancellationToken cancellationToken = default)
		{
			var report = await _dbContext.Reports.FirstOrDefaultAsync(report => report.Id == request.Id);
			if (report == null)
			{
				throw new ClearReportNotFoundException();
			}
			report.Cleared = true;
			_dbContext.Reports.Update(report);
			await _dbContext.SaveChangesAsync(cancellationToken);
			return Unit.Value;
		}
	}
}
