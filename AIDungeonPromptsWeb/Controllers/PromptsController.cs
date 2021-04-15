using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.Identity;
using AIDungeonPrompts.Application.Commands.CreatePrompt;
using AIDungeonPrompts.Application.Commands.CreateTransientUser;
using AIDungeonPrompts.Application.Commands.DeletePrompt;
using AIDungeonPrompts.Application.Commands.UpdatePrompt;
using AIDungeonPrompts.Application.Helpers;
using AIDungeonPrompts.Application.Queries.GetPrompt;
using AIDungeonPrompts.Application.Queries.GetScript;
using AIDungeonPrompts.Application.Queries.GetServerFlag;
using AIDungeonPrompts.Application.Queries.GetUser;
using AIDungeonPrompts.Application.Queries.SimilarPrompt;
using AIDungeonPrompts.Domain.Entities;
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
		public async Task<ActionResult> Create(int? parentId, CancellationToken cancellationToken)
		{
			var flag = await _mediator.Send(new GetServerFlagQuery(ServerFlagName.CreateDisabled), cancellationToken);
			return View(new CreatePromptViewModel
			{
				Command = new CreatePromptCommand
				{
					ParentId = parentId
				},
				CreationDisabled = flag.Enabled,
				DisabledMessage = flag.Message
			});
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(bool addWi, bool confirm, bool saveDraft, bool addChild, bool uploadWi, int? wiDelete, CreatePromptViewModel model, CancellationToken cancellationToken)
		{
			var flag = await _mediator.Send(new GetServerFlagQuery(ServerFlagName.CreateDisabled), cancellationToken);
			if (flag.Enabled)
			{
				return RedirectToAction("Create");
			}

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
				await model.ScriptZip.CopyToAsync(stream, cancellationToken);
				model.Command.ScriptZip = stream.ToArray();

				var validator = new CreatePromptCommandValidator();
				var results = await validator.ValidateAsync(model.Command, cancellationToken);

				results.AddToModelState(ModelState, nameof(model.Command));
			}

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			if (!model.Command.SaveDraft && !addChild && !model.Command.ParentId.HasValue)
			{
				var duplicate = await _mediator.Send(new SimilarPromptQuery(model.Command.Title), cancellationToken);
				if (duplicate.Matched && !confirm)
				{
					model.SimilarPromptQuery = duplicate;
					return View(model);
				}
			}

			if (!_currentUserService.TryGetCurrentUser(out var user))
			{
				var userId = await _mediator.Send(new CreateTransientUserCommand(), cancellationToken);
				user = await _mediator.Send(new GetUserQuery(userId), cancellationToken);
				await HttpContext.SignInUserAsync(user);
				model.Command.OwnerId = userId;
			}
			else
			{
				model.Command.OwnerId = user!.Id;
			}

			var id = await _mediator.Send(model.Command, cancellationToken);

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
		public async Task<ActionResult> Delete(int? id, CancellationToken cancellationToken)
		{
			if (id == null || !_currentUserService.TryGetCurrentUser(out var user))
			{
				return NotFound();
			}

			var prompt = await _mediator.Send(new GetPromptQuery(id.Value), cancellationToken);

			if (prompt == null || (prompt.OwnerId != user!.Id && (user.Role & RoleEnum.Delete) == 0))
			{
				return NotFound();
			}

			await _mediator.Send(new DeletePromptCommand(prompt.Id), cancellationToken);

			if (prompt.ParentId.HasValue)
			{
				return RedirectToAction("Edit", new { id = prompt.ParentId });
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpGet("/{id}/world-info")]
		public async Task<IActionResult> DownloadWorldInfo(int? id, CancellationToken cancellationToken)
		{
			if (id == null || id == default)
			{
				return NotFound();
			}
			var prompt = await _mediator.Send(new GetPromptQuery(id.Value), cancellationToken);
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
		public async Task<IActionResult> Edit(int? id, bool addWi, bool saveDraft, bool confirm, bool addChild, bool uploadWi, int? wiDelete, UpdatePromptViewModel model, CancellationToken cancellationToken)
		{
			model.Command.SaveDraft = saveDraft;

			if (id == null || !_currentUserService.TryGetCurrentUser(out var user))
			{
				return NotFound();
			}

			var prompt = await _mediator.Send(new GetPromptQuery(id.Value), cancellationToken);

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

			if (model.ScriptZip != null)
			{
				using var stream = new MemoryStream();
				await model.ScriptZip.CopyToAsync(stream, cancellationToken);
				model.Command.ScriptZip = stream.ToArray();

				var validator = new UpdatePromptCommandValidator();
				var results = await validator.ValidateAsync(model.Command, cancellationToken);

				results.AddToModelState(ModelState, nameof(model.Command));
			}

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			if (!model.Command.SaveDraft && !addChild && !model.Command.ParentId.HasValue)
			{
				var duplicate = await _mediator.Send(new SimilarPromptQuery(model.Command.Title, model.Command.Id), cancellationToken);
				if (duplicate.Matched && !confirm)
				{
					model.SimilarPromptQuery = duplicate;
					return View(model);
				}
			}

			await _mediator.Send(model.Command, cancellationToken);

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
		public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
		{
			if (id == null || !_currentUserService.TryGetCurrentUser(out var user))
			{
				return NotFound();
			}

			var prompt = await _mediator.Send(new GetPromptQuery(id.Value), cancellationToken);

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
		public async Task<ActionResult<GetPromptViewModel>> Get(int? id, CancellationToken cancellationToken)
		{
			if (id == null || id == default)
			{
				return NotFound();
			}
			var prompt = await _mediator.Send(new GetPromptQuery(id.Value), cancellationToken);
			return prompt ?? (ActionResult<GetPromptViewModel>)NotFound();
		}

		[HttpGet("{id}/report")]
		public async Task<IActionResult> Report(int? id, CancellationToken cancellationToken)
		{
			if (id == null || id == default)
			{
				return NotFound();
			}
			var prompt = await _mediator.Send(new GetPromptQuery(id.Value), cancellationToken);
			if (prompt == null)
			{
				return NotFound();
			}
			return View(new CreateReportViewModel { Prompt = prompt });
		}

		[HttpPost("{id}/report"), ValidateAntiForgeryToken]
		public async Task<IActionResult> Report(int? id, CreateReportViewModel viewModel, CancellationToken cancellationToken)
		{
			if (id == null || id == default)
			{
				return NotFound();
			}
			var prompt = await _mediator.Send(new GetPromptQuery(id.Value), cancellationToken);
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
			await _mediator.Send(viewModel.Command, cancellationToken);
			return RedirectToAction("View", new { id, reported = true });
		}

		[HttpGet("{id}/script")]
		public async Task<ActionResult> Script(int? id, CancellationToken cancellationToken)
		{
			if (id == null)
			{
				return BadRequest();
			}

			var file = await _mediator.Send(new GetScriptQuery(id.Value), cancellationToken);

			if (file == null)
			{
				return NotFound();
			}

			Stream stream = new MemoryStream(file);
			const string? mimeType = "application/zip";
			return new FileStreamResult(stream, mimeType)
			{
				FileDownloadName = $"{id}-scripts.zip"
			};
		}

		[HttpGet("/{id}")]
		public async Task<IActionResult> View(int? id, bool? reported, CancellationToken cancellationToken)
		{
			if (id == null || id == default)
			{
				return NotFound();
			}
			var prompt = await _mediator.Send(new GetPromptQuery(id.Value), cancellationToken);
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
