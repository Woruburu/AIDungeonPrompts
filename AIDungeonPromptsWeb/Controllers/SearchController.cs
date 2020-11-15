using System.Threading.Tasks;
using AIDungeonPrompts.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AIDungeonPrompts.Web.Controllers
{
	public class SearchController : Controller
	{
		private readonly IMediator _mediator;

		public SearchController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet("[controller]")]
		public async Task<ActionResult> Index(SearchRequestParameters request)
		{
			return RedirectToActionPermanent("Index", "Home", new { request.NsfwSetting, request.Page, request.Query, request.Reverse, request.Tags });
		}
	}
}
