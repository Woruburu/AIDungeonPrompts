using System.Threading.Tasks;
using AIDungeonPrompts.Application.Commands.CreatePrompt;
using AIDungeonPrompts.Application.Queries.GetPrompt;
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
			return View(new CreatePromptCommand());
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(bool addWi, string? honey, CreatePromptCommand command)
		{
			if (!string.IsNullOrWhiteSpace(honey))
			{
				return View(command);
			}

			if (addWi)
			{
				ModelState.Clear();
				command.WorldInfos.Add(new CreatePromptCommandWorldInfo());
				return View(command);
			}

			if (!ModelState.IsValid)
			{
				return View(command);
			}

			var id = await _mediator.Send(command);
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
		public async Task<IActionResult> View(int id, bool? reported)
		{
			if (id == default)
			{
				return NotFound();
			}
			var prompt = await _mediator.Send(new GetPromptQuery { Id = id });
			return View(new ViewPromptViewModel { Prompt = prompt, Reported = reported });
		}

		[HttpGet("[controller]/{id}")]
		public IActionResult ViewOld(int id)
		{
			return RedirectToActionPermanent("View", new { id });
		}
	}
}
