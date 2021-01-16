using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Application.Abstractions.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIDungeonPrompts.Application.Queries.GetPrompt
{
	public class GetPromptQuery : IRequest<GetPromptViewModel?>
	{
		public GetPromptQuery(int id)
		{
			Id = id;
		}

		public int Id { get; set; }
	}

	public class GetPromptQueryHandler : IRequestHandler<GetPromptQuery, GetPromptViewModel?>
	{
		private readonly IAIDungeonPromptsDbContext _dbContext;
		private readonly ICurrentUserService _userService;

		public GetPromptQueryHandler(IAIDungeonPromptsDbContext dbContext, ICurrentUserService userService)
		{
			_dbContext = dbContext;
			_userService = userService;
		}

		public async Task<GetPromptViewModel?> Handle(GetPromptQuery request, CancellationToken cancellationToken = default)
		{
			var isDraft = true;
			if (!(_dbContext is DbContext dbContext))
			{
				throw new Exception($"{nameof(_dbContext)} was not a DbContext");
			}

			var findParent = await _dbContext
				.Prompts
				.Include(e => e.Parent)
				.FirstOrDefaultAsync(e => e.Id == request.Id);
			if (findParent == null)
			{
				return null;
			}
			while (findParent!.ParentId != null)
			{
				await dbContext
					.Entry(findParent)
					.Reference(e => e.Parent)
					.LoadAsync();
				findParent = findParent.Parent;
			}
			isDraft = findParent.IsDraft;

			var prompt = await _dbContext.Prompts
				.Include(e => e.WorldInfos)
				.Include(e => e.PromptTags)
				.ThenInclude(e => e.Tag)
				.Include(e => e.Children)
				.AsNoTracking()
				.Select(prompt => new GetPromptViewModel
				{
					AuthorsNote = prompt.AuthorsNote,
					Id = prompt.Id,
					Memory = prompt.Memory,
					Nsfw = prompt.Nsfw,
					PromptContent = prompt.PromptContent,
					Quests = prompt.Quests,
					Title = prompt.Title,
					Description = prompt.Description,
					DateCreated = prompt.DateCreated,
					ParentId = prompt.ParentId,
					OwnerId = prompt.OwnerId,
					WorldInfos = prompt.WorldInfos.Select(worldInfo => new GetPromptWorldInfoViewModel
					{
						Id = worldInfo.Id,
						Entry = worldInfo.Entry,
						Keys = worldInfo.Keys
					}),
					PromptTags = prompt.PromptTags.Select(promptTag => new GetPromptPromptTagViewModel
					{
						Id = promptTag.Tag!.Id,
						Name = promptTag.Tag!.Name
					}),
					IsDraft = prompt.IsDraft,
					Children = prompt.Children.Select(child => new GetPromptChild
					{
						Id = child.Id,
						Title = child.Title
					})
				}).FirstOrDefaultAsync(prompt => prompt.Id == request.Id);

			if (prompt == null || (isDraft && (!_userService.TryGetCurrentUser(out var user) || prompt.OwnerId != user!.Id)))
			{
				return null;
			}
			return prompt;
		}
	}
}
