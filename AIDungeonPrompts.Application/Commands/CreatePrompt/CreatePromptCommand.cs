using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIDungeonPrompts.Application.Commands.CreatePrompt
{
	public class CreatePromptCommand : IRequest<int>
	{
		[Display(Name = "Authors Note")]
		public string? AuthorsNote { get; set; }

		public string? Description { get; set; }
		public string? Memory { get; set; }

		[Display(Name = "NSFW?")]
		public bool Nsfw { get; set; }

		[Display(Name = "Prompt")]
		public string PromptContent { get; set; } = string.Empty;

		[Display(Name = "Tags (comma delimited)"), Required(ErrorMessage = "Please supply at least a single tag")]
		public string PromptTags { get; set; } = string.Empty;

		public string? Quests { get; set; }

		public string Title { get; set; } = string.Empty;

		[Display(Name = "World Info's")]
		public List<CreatePromptCommandWorldInfo> WorldInfos { get; set; } = new List<CreatePromptCommandWorldInfo>()
		{
			new CreatePromptCommandWorldInfo(),
		};
	}

	public class CreatePromptCommandHandler : IRequestHandler<CreatePromptCommand, int>
	{
		private readonly IAIDungeonPromptsDbContext _dbContext;

		public CreatePromptCommandHandler(IAIDungeonPromptsDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<int> Handle(CreatePromptCommand request, CancellationToken cancellationToken)
		{
			var prompt = new Prompt
			{
				AuthorsNote = request.AuthorsNote,
				DateCreated = DateTime.Now,
				DateEdited = null,
				Memory = request.Memory,
				Nsfw = request.Nsfw,
				PromptContent = request.PromptContent,
				Quests = request.Quests,
				Title = request.Title,
				Description = request.Description,
				Upvote = 0,
				Views = 0
			};

			foreach (var worldInfo in request.WorldInfos)
			{
				if (string.IsNullOrWhiteSpace(worldInfo.Entry) || string.IsNullOrWhiteSpace(worldInfo.Keys))
				{
					continue;
				}
				prompt.WorldInfos.Add(new WorldInfo
				{
					DateCreated = DateTime.Now,
					Entry = worldInfo.Entry,
					Keys = worldInfo.Keys,
					Prompt = prompt
				});
			}

			foreach (var promptTagValue in request.PromptTags.Split(','))
			{
				var promptTag = promptTagValue.Trim();
				var tag = await _dbContext.Tags.FirstOrDefaultAsync(e => e.Name == promptTag);
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
