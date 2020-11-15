using System.Threading.Tasks;
using AIDungeonPrompts.Application.Commands.CreatePrompt;
using AIDungeonPrompts.Application.Commands.CreateReport;
using AIDungeonPrompts.Application.Queries.GetPrompt;
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
		public async Task<ActionResult> Create(bool addWi, string? honey, CreatePromptCommand command)
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
		public async Task<ActionResult> Report(int id)
		{
			ViewData["prompt"] = await _mediator.Send(new GetPromptQuery { Id = id });
			return View(new CreateReportCommand { PromptId = id });
		}

		[HttpPost("{id}/report")]
		public async Task<ActionResult> Report(int id, CreateReportCommand command)
		{
			command.PromptId = id;
			await _mediator.Send(command);
			return RedirectToAction("View", new { id, reported = true });
		}

		[HttpGet("/{id}")]
		public async Task<ActionResult> View(int id, bool? reported)
		{
			ViewData["reported"] = reported ?? false;
			var prompt = await _mediator.Send(new GetPromptQuery { Id = id });
			return View(prompt);
		}

		[HttpGet("[controller]/{id}")]
		public ActionResult ViewOld(int id)
		{
			return RedirectToActionPermanent("View", new { id });
		}
	}
}
