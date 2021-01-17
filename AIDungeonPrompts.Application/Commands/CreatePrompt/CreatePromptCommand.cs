using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Application.Abstractions.Identity;
using AIDungeonPrompts.Application.Helpers;
using AIDungeonPrompts.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIDungeonPrompts.Application.Commands.CreatePrompt
{
	public class CreatePromptCommand : IRequest<int>
	{
		[Display(Name = "Author's Note")]
		public string? AuthorsNote { get; set; }

		public string? Description { get; set; }
		public string? Memory { get; set; }

		[Display(Name = "NSFW?")]
		public bool Nsfw { get; set; }

		public int OwnerId { get; set; }

		public int? ParentId { get; set; }

		[Display(Name = "Prompt")]
		public string PromptContent { get; set; } = string.Empty;

		[Display(Name = "Tags (comma delimited)")]
		public string PromptTags { get; set; } = string.Empty;

		public string? Quests { get; set; }

		public bool SaveDraft { get; set; }

		public string Title { get; set; } = string.Empty;

		[Display(Name = "World Info")]
		public List<CreatePromptCommandWorldInfo> WorldInfos { get; set; } = new List<CreatePromptCommandWorldInfo>()
		{
			new CreatePromptCommandWorldInfo(),
		};
	}

	public class CreatePromptCommandHandler : IRequestHandler<CreatePromptCommand, int>
	{
		private readonly ICurrentUserService _currentUserService;
		private readonly IAIDungeonPromptsDbContext _dbContext;

		public CreatePromptCommandHandler(IAIDungeonPromptsDbContext dbContext, ICurrentUserService currentUserService)
		{
			_dbContext = dbContext;
			_currentUserService = currentUserService;
		}

		public async Task<int> Handle(CreatePromptCommand request, CancellationToken cancellationToken = default)
		{
			if (request.ParentId.HasValue)
			{
				if (!_currentUserService.TryGetCurrentUser(out var user))
				{
					throw new CreatePromptUnauthorizedParentException();
				}
				var parent = await _dbContext.Prompts.FindAsync(request.ParentId);
				if (parent.OwnerId != user!.Id)
				{
					throw new CreatePromptUnauthorizedParentException();
				}
			}

			var isDraft = request.ParentId.HasValue ? false : request.SaveDraft;

			var prompt = new Prompt
			{
				AuthorsNote = request.AuthorsNote?.Replace("\r\n", "\n"),
				DateCreated = DateTime.UtcNow,
				DateEdited = null,
				Memory = request.Memory?.Replace("\r\n", "\n"),
				Nsfw = request.Nsfw,
				PromptContent = request.PromptContent.Replace("\r\n", "\n"),
				Quests = request.Quests?.Replace("\r\n", "\n"),
				Title = request.Title.Replace("\r\n", "\n"),
				Description = request.Description?.Replace("\r\n", "\n"),
				OwnerId = request.OwnerId,
				Upvote = 0,
				Views = 0,
				IsDraft = isDraft,
				PublishDate = isDraft ? null : (DateTime?)DateTime.UtcNow,
				ParentId = request.ParentId
			};

			foreach (var worldInfo in request.WorldInfos)
			{
				if (string.IsNullOrWhiteSpace(worldInfo.Entry) || string.IsNullOrWhiteSpace(worldInfo.Keys))
				{
					continue;
				}
				prompt.WorldInfos.Add(new WorldInfo
				{
					DateCreated = DateTime.UtcNow,
					Entry = worldInfo.Entry.Replace("\r\n", "\n"),
					Keys = worldInfo.Keys.Replace("\r\n", "\n"),
					Prompt = prompt
				});
			}

			var promptTags = request.PromptTags.Split(',').Select(p => p.Trim().ToLower()).Distinct();
			foreach (var promptTag in promptTags)
			{
				if (string.IsNullOrWhiteSpace(promptTag))
				{
					continue;
				}
				if (string.Equals(promptTag, "nsfw", StringComparison.OrdinalIgnoreCase))
				{
					prompt.Nsfw = true;
					continue;
				}
				var tag = await _dbContext.Tags.FirstOrDefaultAsync(e => EF.Functions.ILike(e.Name, NpgsqlHelper.SafeIlike(promptTag), NpgsqlHelper.EscapeChar));
				if (tag == null)
				{
					prompt.PromptTags.Add(new PromptTag { Prompt = prompt, Tag = new Tag { Name = promptTag } });
				}
				else
				{
					prompt.PromptTags.Add(new PromptTag { Prompt = prompt, Tag = tag });
				}
			}

			_dbContext.Prompts.Add(prompt);
			await _dbContext.SaveChangesAsync(cancellationToken);
			return prompt.Id;
		}
	}
}
