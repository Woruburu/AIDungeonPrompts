using System.Diagnostics;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Queries.SearchPrompts;
using AIDungeonPrompts.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AIDungeonPrompts.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly IMediator _mediator;

		public HomeController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		public async Task<ActionResult<SearchPromptsViewModel>> Index()
		{
			var results = await _mediator.Send(new SearchPromptsQuery { Nsfw = SearchNsfw.Both });
			return View(results);
		}
	}
}
