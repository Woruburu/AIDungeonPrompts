using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIDungeonPrompts.Application.Queries.RandomPrompt
{
	public class RandomPromptQuery : IRequest<RandomPromptViewModel?>
	{
	}

	public class RandomPromptQueryHandler : IRequestHandler<RandomPromptQuery, RandomPromptViewModel?>
	{
		private readonly IAIDungeonPromptsDbContext _dbContext;

		public RandomPromptQueryHandler(IAIDungeonPromptsDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		[SuppressMessage("Security", "SCS0005:Weak random generator",
			Justification = "Cryptographic Generator not required")]
		public async Task<RandomPromptViewModel?> Handle(RandomPromptQuery request,
			CancellationToken cancellationToken = default)
		{
			var count = await _dbContext.Prompts.Where(e => !e.IsDraft).CountAsync();
			if (count < 1)
			{
				return null;
			}

			var value = new Random().Next(count);
			var id = await _dbContext
				.Prompts
				.Where(e => !e.IsDraft)
				.OrderBy(e => e.Id)
				.Skip(value)
				.Take(1)
				.AsNoTracking()
				.Select(e => e.Id)
				.FirstOrDefaultAsync();

			if (id == default)
			{
				return null;
			}

			return new RandomPromptViewModel {Id = id};
		}
	}
}
