using System.Threading.Tasks;
using AIDungeonPrompts.Application.Commands.CreatePrompt;
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
		public async Task<ActionResult> Create(bool addWi, string honey, CreatePromptCommand command)
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

		[HttpGet("[controller]/{id}")]
		public async Task<ActionResult> View(int id)
		{
			var prompt = await _mediator.Send(new GetPromptQuery { Id = id });
			return View(prompt);
		}
	}
}
