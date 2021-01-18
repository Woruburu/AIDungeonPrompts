using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.Identity;
using AIDungeonPrompts.Application.Commands.CreatePrompt;
using AIDungeonPrompts.Application.Commands.CreateTransientUser;
using AIDungeonPrompts.Application.Commands.DeletePrompt;
using AIDungeonPrompts.Application.Commands.UpdatePrompt;
using AIDungeonPrompts.Application.Helpers;
using AIDungeonPrompts.Application.Queries.GetPrompt;
using AIDungeonPrompts.Application.Queries.GetUser;
using AIDungeonPrompts.Application.Queries.SimilarPrompt;
using AIDungeonPrompts.Domain.Enums;
using AIDungeonPrompts.Web.Extensions;
using AIDungeonPrompts.Web.Models.Prompts;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AIDungeonPrompts.Web.Controllers
{
	public class PromptsController : Controller
	{
		private readonly ICurrentUserService _currentUserService;
		private readonly ILogger<PromptsController> _logger;
		private readonly IMediator _mediator;

		public PromptsController(IMediator mediator, ICurrentUserService currentUserService, ILogger<PromptsController> logger)
		{
			_mediator = mediator;
			_currentUserService = currentUserService;
			_logger = logger;
		}

		[HttpGet("[controller]/create")]
		public ActionResult Create(int? parentId)
		{
			return View(new CreatePromptViewModel
			{
				Command = new CreatePromptCommand
				{
					ParentId = parentId
				}
			});
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(bool addWi, bool confirm, bool saveDraft, bool addChild, bool uploadWi, int? wiDelete, CreatePromptViewModel model)
		{
			if (uploadWi)
			{
				ModelState.Clear();
				if (model.WorldInfoFile != null)
				{
					var worldInfos = await ReadWorldInfoFromFileAsync(model.WorldInfoFile);
					if (worldInfos?.Count > 0)
					{
						if (model.Command.WorldInfos.Count == 1
							&& string.IsNullOrWhiteSpace(model.Command.WorldInfos[0].Entry)
							&& string.IsNullOrWhiteSpace(model.Command.WorldInfos[0].Keys))
						{
							model.Command.WorldInfos = worldInfos.Select(wi => new CreatePromptCommandWorldInfo
							{
								Keys = wi.Keys,
								Entry = wi.Entry
							}).ToList();
						}
						else
						{
							model.Command.WorldInfos.AddRange(worldInfos.Select(wi => new CreatePromptCommandWorldInfo
							{
								Keys = wi.Keys,
								Entry = wi.Entry
							}));
						}
					}
				}
				return View(model);
			}

			if (wiDelete.HasValue)
			{
				ModelState.Clear();
				model.Command.WorldInfos.RemoveAt(wiDelete.Value);
				if (model.Command.WorldInfos.Count < 1)
				{
					model.Command.WorldInfos.Add(new CreatePromptCommandWorldInfo());
				}
				return View(model);
			}

			if (addWi)
			{
				ModelState.Clear();
				model.Command.WorldInfos.Add(new CreatePromptCommandWorldInfo());
				return View(model);
			}

			model.Command.SaveDraft = saveDraft || addChild;

			if (model.ScriptZip != null)
			{
				using var stream = new MemoryStream();
				await model.ScriptZip.CopyToAsync(stream);
				model.Command.ScriptZip = stream.ToArray();

				var validator = new CreatePromptCommandValidator();
				var results = validator.Validate(model.Command);

				results.AddToModelState(ModelState, nameof(model.Command));
			}

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			if (!model.Command.SaveDraft && !addChild && !model.Command.ParentId.HasValue)
			{
				var duplicate = await _mediator.Send(new SimilarPromptQuery(model.Command.Title));
				if (duplicate.Matched && !confirm)
				{
					model.SimilarPromptQuery = duplicate;
					return View(model);
				}
			}

			if (!_currentUserService.TryGetCurrentUser(out var user))
			{
				var userId = await _mediator.Send(new CreateTransientUserCommand());
				user = await _mediator.Send(new GetUserQuery(userId));
				await HttpContext.SignInUserAsync(user);
				model.Command.OwnerId = userId;
			}
			else
			{
				model.Command.OwnerId = user!.Id;
			}

			var id = await _mediator.Send(model.Command);

			if (addChild)
			{
				return RedirectToAction("Create", new { parentId = id });
			}

			if (model.Command.ParentId.HasValue)
			{
				return RedirectToAction("Edit", new { id = model.Command.ParentId });
			}

			return RedirectToAction("View", new { id });
		}

		[HttpPost("/{id}/delete"), ValidateAntiForgeryToken, Authorize]
		public async Task<ActionResult> Delete(int? id)
		{
			if (id == null || !_currentUserService.TryGetCurrentUser(out var user))
			{
				return NotFound();
			}

			var prompt = await _mediator.Send(new GetPromptQuery(id.Value));

			if (prompt == null || (prompt.OwnerId != user!.Id && (user.Role & RoleEnum.Delete) == 0))
			{
				return NotFound();
			}

			await _mediator.Send(new DeletePromptCommand(prompt.Id));

			if (prompt.ParentId.HasValue)
			{
				return RedirectToAction("Edit", new { id = prompt.ParentId });
			}

			return RedirectToAction("Index");
		}

		[HttpGet("/{id}/world-info")]
		public async Task<IActionResult> DownloadWorldInfo(int? id)
		{
			if (id == null || id == default)
			{
				return NotFound();
			}
			var prompt = await _mediator.Send(new GetPromptQuery(id.Value));
			if (prompt == null)
			{
				return NotFound();
			}
			var worldInfos = prompt.WorldInfos.Select(wi => new WorldInfoJson
			{
				Entry = wi.Entry,
				Keys = wi.Keys,
				IsNotHidden = true
			});
			var worldInfosString = JsonSerializer.Serialize(worldInfos, new JsonSerializerOptions
			{
				WriteIndented = true,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			});
			Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(worldInfosString));
			const string? mimeType = "application/json";
			return new FileStreamResult(stream, mimeType)
			{
				FileDownloadName = "worldInfo.json"
			};
		}

		[HttpPost("/{id}/edit"), ValidateAntiForgeryToken, Authorize]
		public async Task<IActionResult> Edit(int? id, bool addWi, bool saveDraft, bool confirm, bool addChild, bool uploadWi, int? wiDelete, UpdatePromptViewModel model)
		{
			model.Command.SaveDraft = saveDraft;

			if (id == null || !_currentUserService.TryGetCurrentUser(out var user))
			{
				return NotFound();
			}

			var prompt = await _mediator.Send(new GetPromptQuery(id.Value));

			if (prompt == null || (prompt.OwnerId != user!.Id && !RoleHelper.CanEdit(user.Role)))
			{
				return NotFound();
			}

			model.Children = prompt.Children.ToList();
			model.Command.Id = prompt.Id;
			model.Command.OwnerId = prompt.OwnerId;

			if (uploadWi)
			{
				ModelState.Clear();
				if (model.WorldInfoFile != null)
				{
					var worldInfos = await ReadWorldInfoFromFileAsync(model.WorldInfoFile);
					if (worldInfos?.Count > 0)
					{
						if (model.Command.WorldInfos.Count == 1
							&& string.IsNullOrWhiteSpace(model.Command.WorldInfos[0].Entry)
							&& string.IsNullOrWhiteSpace(model.Command.WorldInfos[0].Keys))
						{
							model.Command.WorldInfos = worldInfos.Select(wi => new UpdatePromptCommandWorldInfo
							{
								Keys = wi.Keys,
								Entry = wi.Entry
							}).ToList();
						}
						else
						{
							model.Command.WorldInfos.AddRange(worldInfos.Select(wi => new UpdatePromptCommandWorldInfo
							{
								Keys = wi.Keys,
								Entry = wi.Entry
							}));
						}
					}
				}
				return View(model);
			}

			if (wiDelete.HasValue)
			{
				ModelState.Clear();
				model.Command.WorldInfos.RemoveAt(wiDelete.Value);
				if (model.Command.WorldInfos.Count < 1)
				{
					model.Command.WorldInfos.Add(new UpdatePromptCommandWorldInfo());
				}
				return View(model);
			}

			if (addWi)
			{
				ModelState.Clear();
				model.Command.WorldInfos.Add(new UpdatePromptCommandWorldInfo());
				return View(model);
			}

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			if (!model.Command.SaveDraft && !addChild && !model.Command.ParentId.HasValue)
			{
				var duplicate = await _mediator.Send(new SimilarPromptQuery(model.Command.Title, model.Command.Id));
				if (duplicate.Matched && !confirm)
				{
					model.SimilarPromptQuery = duplicate;
					return View(model);
				}
			}

			await _mediator.Send(model.Command);

			if (addChild)
			{
				return RedirectToAction("Create", new { parentId = id });
			}

			if (model.Command.ParentId.HasValue)
			{
				return RedirectToAction("Edit", new { id = model.Command.ParentId });
			}

			return RedirectToAction("View", new { id });
		}

		[HttpGet("/{id}/edit"), Authorize]
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || !_currentUserService.TryGetCurrentUser(out var user))
			{
				return NotFound();
			}

			var prompt = await _mediator.Send(new GetPromptQuery(id.Value));

			if (prompt == null || (prompt.OwnerId != user!.Id && !RoleHelper.CanEdit(user.Role)))
			{
				return NotFound();
			}

			return View(new UpdatePromptViewModel
			{
				Children = prompt.Children.ToList(),
				Command = new UpdatePromptCommand
				{
					AuthorsNote = prompt.AuthorsNote,
					Description = prompt.Description,
					Id = prompt.Id,
					Memory = prompt.Memory,
					Nsfw = prompt.Nsfw,
					PromptContent = prompt.PromptContent,
					PromptTags = string.Join(", ", prompt.PromptTags.Select(pt => pt.Name)),
					Quests = prompt.Quests,
					Title = prompt.Title,
					OwnerId = prompt.OwnerId,
					ParentId = prompt.ParentId,
					WorldInfos = prompt.WorldInfos.Any()
						? prompt.WorldInfos.Select(wi => new UpdatePromptCommandWorldInfo
						{
							Entry = wi.Entry,
							Keys = wi.Keys
						}).ToList()
						: new List<UpdatePromptCommandWorldInfo>()
						{
							 new UpdatePromptCommandWorldInfo()
						}
				}
			});
		}

		[EnableCors("AiDungeon"), HttpGet("/api/{id}")]
		public async Task<ActionResult<GetPromptViewModel>> Get(int? id)
		{
			if (id == null || id == default)
			{
				return NotFound();
			}
			var prompt = await _mediator.Send(new GetPromptQuery(id.Value));
			return prompt ?? (ActionResult<GetPromptViewModel>)NotFound();
		}

		[HttpGet("{id}/report")]
		public async Task<IActionResult> Report(int? id)
		{
			if (id == null || id == default)
			{
				return NotFound();
			}
			var prompt = await _mediator.Send(new GetPromptQuery(id.Value));
			if (prompt == null)
			{
				return NotFound();
			}
			return View(new CreateReportViewModel { Prompt = prompt });
		}

		[HttpPost("{id}/report"), ValidateAntiForgeryToken]
		public async Task<IActionResult> Report(int? id, CreateReportViewModel viewModel)
		{
			if (id == null || id == default)
			{
				return NotFound();
			}
			var prompt = await _mediator.Send(new GetPromptQuery(id.Value));
			if (prompt == null)
			{
				return NotFound();
			}
			if (!ModelState.IsValid)
			{
				viewModel.Prompt = prompt;
				return View(viewModel);
			}
			viewModel.Command.PromptId = id.Value;
			await _mediator.Send(viewModel.Command);
			return RedirectToAction("View", new { id, reported = true });
		}

		[HttpGet("/{id}")]
		public async Task<IActionResult> View(int? id, bool? reported)
		{
			if (id == null || id == default)
			{
				return NotFound();
			}
			var prompt = await _mediator.Send(new GetPromptQuery(id.Value));
			if (prompt == null)
			{
				return NotFound();
			}
			return View(new ViewPromptViewModel { Prompt = prompt, Reported = reported });
		}

		[HttpGet("[controller]/{id}")]
		public IActionResult ViewOld(int? id)
		{
			return RedirectToActionPermanent("View", new { id });
		}

		private async Task<List<WorldInfoJson>> ReadWorldInfoFromFileAsync(IFormFile file)
		{
			var serializerOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			try
			{
				using var stream = file.OpenReadStream();
				using var reader = new StreamReader(stream);
				var fileString = await reader.ReadToEndAsync();
				return JsonSerializer.Deserialize<List<WorldInfoJson>>(fileString, serializerOptions) ?? new List<WorldInfoJson>();
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Could not read World Info from JSON");
				return new List<WorldInfoJson>();
			}
		}
	}
}
