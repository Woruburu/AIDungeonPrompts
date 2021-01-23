using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.Identity;
using AIDungeonPrompts.Application.Commands.ClearReport;
using AIDungeonPrompts.Application.Queries.GetReports;
using AIDungeonPrompts.Web.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIDungeonPrompts.Web.Controllers
{
	[Authorize(Policy = PolicyValueConstants.EditorsOnly)]
	public class ReportsController : Controller
	{
		private readonly ICurrentUserService _currentUserService;
		private readonly IMediator _mediator;

		public ReportsController(IMediator mediator, ICurrentUserService currentUserService)
		{
			_mediator = mediator;
			_currentUserService = currentUserService;
		}

		[HttpPost("[controller]/clear/{id}"), ValidateAntiForgeryToken]
		public async Task<IActionResult> Clear(int? id, CancellationToken cancellationToken)
		{
			if (id == null)
			{
				return NotFound();
			}
			await _mediator.Send(new ClearReportCommand(id.Value), cancellationToken);
			return RedirectToAction("Index");
		}

		[HttpGet("[controller]")]
		public async Task<IActionResult> Index(CancellationToken cancellationToken)
		{
			if (!_currentUserService.TryGetCurrentUser(out var user))
			{
				return NotFound();
			}
			var reports = await _mediator.Send(new GetReportsQuery(user!.Role), cancellationToken);
			return View(reports);
		}
	}
}
