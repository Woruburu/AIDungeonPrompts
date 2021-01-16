using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIDungeonPrompts.Application.Queries.GetAllTags
{
	public class GetAllTagsQuery : IRequest<List<GetTagViewModel>>
	{
	}

	public class GetAllTagsQueryHandler : IRequestHandler<GetAllTagsQuery, List<GetTagViewModel>>
	{
		private readonly IAIDungeonPromptsDbContext _dbContext;

		public GetAllTagsQueryHandler(IAIDungeonPromptsDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public Task<List<GetTagViewModel>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken = default)
		{
			return _dbContext.PromptTags
				.Include(promptTag => promptTag.Tag)
				.Where(promptTag => !promptTag.Prompt!.IsDraft)
				.AsNoTracking()
				.Select(promptTag => new GetTagViewModel
				{
					Id = promptTag.Tag!.Id,
					Name = promptTag.Tag.Name,
					Count = promptTag.Tag.PromptTags.Count
				})
				.Distinct()
				.ToListAsync(cancellationToken);
		}
	}
}
