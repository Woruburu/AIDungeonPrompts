using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using MediatR;

namespace AIDungeonPrompts.Application.Commands.ClaimPrompt
{
	public class ClaimPromptCommand : IRequest
	{
		public int OwnerId { get; set; }
		public int PromptId { get; set; }
	}

	public class ClaimPromptCommandHandler : IRequestHandler<ClaimPromptCommand>
	{
		private readonly IAIDungeonPromptsDbContext _dbContext;

		public ClaimPromptCommandHandler(IAIDungeonPromptsDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<Unit> Handle(ClaimPromptCommand request, CancellationToken cancellationToken)
		{
			var prompt = await _dbContext.Prompts.FindAsync(request.PromptId);
			if (prompt.OwnerId != null)
			{
				return Unit.Value;
			}
			prompt.OwnerId = request.OwnerId;
			_dbContext.Prompts.Update(prompt);
			await _dbContext.SaveChangesAsync(cancellationToken);
			return Unit.Value;
		}
	}
}
