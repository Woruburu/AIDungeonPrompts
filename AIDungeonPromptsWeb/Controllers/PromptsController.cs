using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.Identity;
using AIDungeonPrompts.Application.Commands.CreatePrompt;
using AIDungeonPrompts.Application.Commands.CreateTransientUser;
using AIDungeonPrompts.Application.Commands.UpdatePrompt;
using AIDungeonPrompts.Application.Helpers;
using AIDungeonPrompts.Application.Queries.GetPrompt;
using AIDungeonPrompts.Application.Queries.GetUser;
using AIDungeonPrompts.Application.Queries.SimilarPrompt;
using AIDungeonPrompts.Web.Extensions;
using AIDungeonPrompts.Web.Models.Prompts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIDungeonPrompts.Web.Controllers
{
	public class PromptsController : Controller
	{
		private readonly ICurrentUserService _currentUserService;
		private readonly IMediator _mediator;

		public PromptsController(IMediator mediator, ICurrentUserService currentUserService)
		{
			_mediator = mediator;
			_currentUserService = currentUserService;
		}

		[HttpGet("[controller]/create")]
		public ActionResult Create()
		{
			return View(new CreatePromptViewModel());
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(bool? addWi, bool confirm, CreatePromptViewModel model)
		{
			if (addWi == true)
			{
				ModelState.Clear();
				model.Command.WorldInfos.Add(new CreatePromptCommandWorldInfo());
				return View(model);
			}

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var duplicate = await _mediator.Send(new SimilarPromptQuery(model.Command.Title));
			if (duplicate.Matched && !confirm)
			{
				model.SimilarPromptQuery = duplicate;
				return View(model);
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
			return RedirectToAction("View", new { id });
		}

		[HttpPost("/{id}/edit"), ValidateAntiForgeryToken, Authorize]
		public async Task<IActionResult> Edit(int? id, bool? addWi, bool confirm, UpdatePromptViewModel model)
		{
			if (id == null || !_currentUserService.TryGetCurrentUser(out var user))
			{
				return NotFound();
			}

			var prompt = await _mediator.Send(new GetPromptQuery(id.Value));

			if (prompt?.OwnerId != user!.Id && !RoleHelper.CanEdit(user.Role))
			{
				return NotFound();
			}

			model.Command.Id = id.Value;

			if (addWi != null && addWi.Value)
			{
				ModelState.Clear();
				model.Command.WorldInfos.Add(new UpdatePromptCommandWorldInfo());
				return View(model);
			}

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var duplicate = await _mediator.Send(new SimilarPromptQuery(model.Command.Title, model.Command.Id));
			if (duplicate.Matched && !confirm)
			{
				model.SimilarPromptQuery = duplicate;
				return View(model);
			}

			await _mediator.Send(model.Command);
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

			if (prompt == null || (prompt?.OwnerId != user!.Id && !RoleHelper.CanEdit(user.Role)))
			{
				return NotFound();
			}

			return View(new UpdatePromptViewModel
			{
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
					WorldInfos = prompt.WorldInfos.Any() ? prompt.WorldInfos.Select(wi => new UpdatePromptCommandWorldInfo
					{
						Entry = wi.Entry,
						Keys = wi.Keys
					}).ToList() : new List<UpdatePromptCommandWorldInfo>()
					{
						 new UpdatePromptCommandWorldInfo()
					}
				}
			});
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
	}
}
