using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIDungeonPrompts.Application.Queries.GetScript
{
	public class GetScriptQuery : IRequest<byte[]?>
	{
		public GetScriptQuery(int promptId)
		{
			PromptId = promptId;
		}

		public int PromptId { get; set; }
	}

	public class GetScriptQueryHandler : IRequestHandler<GetScriptQuery, byte[]?>
	{
		private readonly IAIDungeonPromptsDbContext _dbContext;

		public GetScriptQueryHandler(IAIDungeonPromptsDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public Task<byte[]?> Handle(GetScriptQuery request, CancellationToken cancellationToken = default) =>
			_dbContext
				.Prompts
				.Where(e => e.Id == request.PromptId)
				.Select(e => e.ScriptZip)
				.FirstOrDefaultAsync(cancellationToken);
	}
}
