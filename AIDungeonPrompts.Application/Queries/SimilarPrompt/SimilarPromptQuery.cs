using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIDungeonPrompts.Application.Queries.SimilarPrompt
{
	public class SimilarPromptQuery : IRequest<SimilarPromptViewModel>
	{
		public string Title { get; set; } = string.Empty;
	}

	public class SimilarPromptQueryHandler : IRequestHandler<SimilarPromptQuery, SimilarPromptViewModel>
	{
		private readonly IAIDungeonPromptsDbContext _dbContext;

		public SimilarPromptQueryHandler(IAIDungeonPromptsDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<SimilarPromptViewModel> Handle(SimilarPromptQuery request, CancellationToken cancellationToken)
		{
			var similarPrompts = await _dbContext.Prompts
				.Where(prompt => EF.Functions.ILike(prompt.Title, $"{request.Title}"))
				.Select(prompt => new SimilarPromptDetailsViewModel
				{
					Id = prompt.Id,
					Title = prompt.Title
				})
				.ToListAsync();

			if (similarPrompts.Count < 1)
			{
				return new SimilarPromptViewModel
				{
					Matched = false
				};
			}

			return new SimilarPromptViewModel
			{
				Matched = true,
				SimilarPrompts = similarPrompts
			};
		}
	}
}
