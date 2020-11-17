using System.Threading.Tasks;
using AIDungeonPrompts.Application.Commands.CreatePrompt;
using AIDungeonPrompts.Application.Queries.GetPrompt;
using AIDungeonPrompts.Application.Queries.SimilarPrompt;
using AIDungeonPrompts.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AIDungeonPrompts.Web.Controllers
{
	public class PromptsController : Controller
	{
		private readonly IMediator _mediator;

		public PromptsController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet("[controller]/create")]
		public ActionResult Create()
		{
			return View(new CreatePromptViewModel());
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(bool? addWi, string? honey, bool confirm, CreatePromptViewModel model)
		{
			if (!string.IsNullOrWhiteSpace(honey))
			{
				return View(model);
			}

			if (addWi != null && addWi.Value)
			{
				ModelState.Clear();
				model.Command.WorldInfos.Add(new CreatePromptCommandWorldInfo());
				return View(model);
			}

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var duplicate = await _mediator.Send(new SimilarPromptQuery { Title = model.Command.Title });
			if (duplicate.Matched && !confirm)
			{
				model.SimilarPromptQuery = duplicate;
				return View(model);
			}

			var id = await _mediator.Send(model.Command);
			return RedirectToAction("View", new { id });
		}

		[HttpGet("{id}/report")]
		public async Task<IActionResult> Report(int id)
		{
			var prompt = await _mediator.Send(new GetPromptQuery { Id = id });
			return View(new CreateReportViewModel { Prompt = prompt });
		}

		[HttpPost("{id}/report"), ValidateAntiForgeryToken]
		public async Task<IActionResult> Report(int id, string? honey, CreateReportViewModel viewModel)
		{
			if (!string.IsNullOrWhiteSpace(honey) || !ModelState.IsValid)
			{
				viewModel.Prompt = await _mediator.Send(new GetPromptQuery { Id = id });
				return View(viewModel);
			}
			viewModel.Command.PromptId = id;
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
			var prompt = await _mediator.Send(new GetPromptQuery { Id = id.Value });
			return View(new ViewPromptViewModel { Prompt = prompt, Reported = reported });
		}

		[HttpGet("[controller]/{id}")]
		public IActionResult ViewOld(int id)
		{
			return RedirectToActionPermanent("View", new { id });
		}
	}
}
