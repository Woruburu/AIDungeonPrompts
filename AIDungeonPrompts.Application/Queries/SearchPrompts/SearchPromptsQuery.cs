using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Application.Helpers;
using AIDungeonPrompts.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIDungeonPrompts.Application.Queries.SearchPrompts
{
	public class SearchPromptsQuery : IRequest<SearchPromptsViewModel>
	{
		public SearchNsfw Nsfw { get; set; }
		public SearchOrderBy OrderBy { get; set; }
		public int Page { get; set; } = 1;
		public int PageSize { get; set; } = 10;
		public bool Reverse { get; set; }
		public string Search { get; set; } = string.Empty;
		public TagJoin TagJoin { get; set; }
		public List<string> Tags { get; set; } = new List<string>();
		public bool TagsFuzzy { get; set; }
		public int? User { get; set; }
	}

	public class SearchPromptsQueryHandler : IRequestHandler<SearchPromptsQuery, SearchPromptsViewModel>
	{
		private readonly IAIDungeonPromptsDbContext _dbContext;

		public SearchPromptsQueryHandler(IAIDungeonPromptsDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<SearchPromptsViewModel> Handle(SearchPromptsQuery request, CancellationToken cancellationToken)
		{
			var query = _dbContext.Prompts
				.Include(prompt => prompt.PromptTags)
				.ThenInclude(prompt => prompt.Tag)
				.AsNoTracking();

			query = OrderBy(request, query);

			switch (request.Nsfw)
			{
				case SearchNsfw.SafeOnly:
					query = query.Where(e => !e.Nsfw);
					break;

				case SearchNsfw.NsfwOnly:
					query = query.Where(e => e.Nsfw);
					break;
			}

			if (!string.IsNullOrWhiteSpace(request.Search))
			{
				query = query.Where(e => EF.Functions.ILike(e.Title, $"%{NpgsqlHelper.SafeIlike(request.Search)}%", NpgsqlHelper.EscapeChar));
			}

			if (request.Tags.Count > 0)
			{
				if (request.TagsFuzzy)
				{
					request.Tags = request.Tags.Select(t => $"%{t}%").ToList();
				}

				switch (request.TagJoin)
				{
					case TagJoin.And:
						foreach (var item in request.Tags)
						{
							query = query.Where(
								prompt => prompt.PromptTags.Any(
									promptTag => EF.Functions.ILike(promptTag.Tag!.Name, item)
								)
							);
						}
						break;

					case TagJoin.Or:
						query = query.Where(
							prompt => prompt.PromptTags.Any(
								promptTag => request.Tags.Any(tag => EF.Functions.ILike(promptTag.Tag!.Name, tag)
								)
							)
						);
						break;
				}
			}

			var skip = (request.Page - 1) * request.PageSize;

			if (request.User != null)
			{
				query = query.Where(e => e.OwnerId == request.User.Value);
			}

			var results = await query
				.Skip(skip)
				.Take(request.PageSize)
				.Select(prompt => new SearchPromptsResultViewModel
				{
					Id = prompt.Id,
					Title = prompt.Title,
					PromptContent = prompt.PromptContent,
					Nsfw = prompt.Nsfw,
					Description = prompt.Description,
					DateCreated = prompt.DateCreated,
					OwnerId = prompt.OwnerId,
					SearchPromptsTagViewModel = prompt.PromptTags
						.Select(promptTag => new SearchPromptsTagViewModel
						{
							Id = promptTag.Tag!.Id,
							Name = promptTag!.Tag!.Name
						})
				})
				.ToListAsync();

			var count = await query.CountAsync();
			var pageAdd = count % request.PageSize == 0 ? 0 : 1;
			var totalPages = count < 1 ? 1 : (count / request.PageSize) + pageAdd;

			return new SearchPromptsViewModel
			{
				Results = results,
				TotalPages = totalPages
			};
		}

		private static IQueryable<Prompt> OrderBy(SearchPromptsQuery request, IQueryable<Prompt> query)
		{
			switch (request.OrderBy)
			{
				case SearchOrderBy.Newest:
					if (!request.Reverse)
					{
						return query.OrderByDescending(prompt => prompt.DateCreated);
					}
					return query.OrderBy(prompt => prompt.DateCreated);

				case SearchOrderBy.Views:
					if (!request.Reverse)
					{
						return query.OrderByDescending(prompt => prompt.Views);
					}
					return query.OrderBy(prompt => prompt.Views);

				case SearchOrderBy.Rating:
					if (!request.Reverse)
					{
						return query.OrderByDescending(prompt => prompt.Upvote);
					}
					return query.OrderBy(prompt => prompt.Upvote);
			}
			return query;
		}
	}
}
