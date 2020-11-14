using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Queries.SearchPrompts;
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

		public async Task<ActionResult> Index(SearchRequestParameters request)
		{
			var tags = new List<string>();
			if (!string.IsNullOrWhiteSpace(request.Tags))
			{
				tags = request.Tags.Split(',').Select(t => t.Trim()).ToList();
			}
			var nsfwIndex = tags.FindIndex(t => string.Equals("nsfw", t, System.StringComparison.OrdinalIgnoreCase));
			if (nsfwIndex > -1)
			{
				request.NsfwSetting = SearchNsfw.NsfwOnly;
				tags.RemoveAt(nsfwIndex);
			}

			var result = await _mediator.Send(new SearchPromptsQuery
			{
				Page = request.Page ?? 1,
				Reverse = request.Reverse,
				Search = request.Query ?? string.Empty,
				Tags = tags,
				Nsfw = request.NsfwSetting
			});

			return View(new SearchViewModel
			{
				Page = request.Page,
				Query = request.Query,
				Reverse = request.Reverse,
				Tags = request.Tags,
				NsfwSetting = request.NsfwSetting,
				SearchResult = result
			});
		}
	}
}
