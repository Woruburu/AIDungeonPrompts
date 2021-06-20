using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Application.Abstractions.Identity;
using AIDungeonPrompts.Application.Queries.GetUser;
using AIDungeonPrompts.Domain.Views;
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

		public async Task<GetPromptViewModel?> Handle(GetPromptQuery request,
			CancellationToken cancellationToken = default)
		{
			NonDraftPrompt? nonDrafts = await _dbContext.NonDraftPrompts.FirstOrDefaultAsync(e => e.Id == request.Id);

			GetPromptViewModel? prompt = await _dbContext.Prompts
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
					PublishDate = prompt.PublishDate,
					WorldInfos =
						prompt.WorldInfos.Select(worldInfo =>
							new GetPromptWorldInfoViewModel
							{
								Id = worldInfo.Id, Entry = worldInfo.Entry, Keys = worldInfo.Keys
							}),
					PromptTags =
						prompt.PromptTags.Select(promptTag =>
							new GetPromptPromptTagViewModel {Id = promptTag.Tag!.Id, Name = promptTag.Tag!.Name}),
					IsDraft = prompt.IsDraft,
					Children = prompt.Children.Select(child => new GetPromptChild
					{
						Id = child.Id, Title = child.Title
					}),
					HasScriptFile = prompt.ScriptZip != null,
					NovelAiScenario = prompt.NovelAiScenario
				}).FirstOrDefaultAsync(prompt => prompt.Id == request.Id);

			if (prompt == null || (nonDrafts == null && (!_userService.TryGetCurrentUser(out GetUserViewModel? user) ||
			                                             prompt.OwnerId != user!.Id)))
			{
				return null;
			}

			return prompt;
		}
	}
}
