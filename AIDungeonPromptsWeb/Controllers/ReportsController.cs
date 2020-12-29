using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.Identity;
using AIDungeonPrompts.Application.Commands.ClearReport;
using AIDungeonPrompts.Application.Helpers;
using AIDungeonPrompts.Application.Queries.GetReports;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIDungeonPrompts.Web.Controllers
{
	public class ReportsController : Controller
	{
		private readonly ICurrentUserService _currentUserService;
		private readonly IMediator _mediator;

		public ReportsController(IMediator mediator, ICurrentUserService currentUserService)
		{
			_mediator = mediator;
			_currentUserService = currentUserService;
		}

		[HttpPost("[controller]/clear/{id}"), Authorize, ValidateAntiForgeryToken]
		public async Task<IActionResult> Clear(int? id)
		{
			if (!_currentUserService.TryGetCurrentUser(out var user) || !RoleHelper.CanEdit(user!.Role) || id == null)
			{
				return NotFound();
			}
			await _mediator.Send(new ClearReportCommand { Id = id.Value });
			return RedirectToAction("Index");
		}

		[HttpGet("[controller]"), Authorize]
		public async Task<IActionResult> Index()
		{
			if (!_currentUserService.TryGetCurrentUser(out var user) || !RoleHelper.CanEdit(user!.Role))
			{
				return NotFound();
			}
			var reports = await _mediator.Send(new GetReportsQuery());
			return View(reports);
		}
	}
}
