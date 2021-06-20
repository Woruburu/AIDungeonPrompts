using AIDungeonPrompts.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace AIDungeonPrompts.Web.Controllers
{
	public class SearchController : Controller
	{
		[HttpGet("[controller]")]
		public ActionResult Index(SearchRequestParameters request) => RedirectToActionPermanent("Index", "Home",
			new
			{
				request.NsfwSetting,
				request.Page,
				request.Query,
				request.Reverse,
				request.Tags
			});
	}
}
