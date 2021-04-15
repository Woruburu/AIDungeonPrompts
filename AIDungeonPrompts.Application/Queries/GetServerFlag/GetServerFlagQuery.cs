using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Application.Queries.GetScript;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIDungeonPrompts.Application.Queries.GetServerFlag
{
	public class GetServerFlagQuery : IRequest<GetServerFlagViewModel>
	{
		public GetServerFlagQuery(string name)
		{
			Name = name;
		}

		public string Name { get; set; }
	}

	public class GetServerFlagQueryHandler : IRequestHandler<GetServerFlagQuery, GetServerFlagViewModel>
	{
		private readonly IAIDungeonPromptsDbContext _dbContext;

		public GetServerFlagQueryHandler(IAIDungeonPromptsDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<GetServerFlagViewModel> Handle(GetServerFlagQuery request, CancellationToken cancellationToken)
		{
			var flag = await _dbContext.ServerFlags.FirstOrDefaultAsync(e => e.Name == request.Name, cancellationToken);
			return flag == null ? new GetServerFlagViewModel() : new GetServerFlagViewModel{Enabled = flag.Enabled, Message = flag.AdditionalMessage};
		}
	}

	public class GetServerFlagViewModel
	{
		public bool Enabled { get; set; }
		public string? Message { get; set; }
	}
}
