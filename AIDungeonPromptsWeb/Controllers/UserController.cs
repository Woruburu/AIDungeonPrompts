using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.Identity;
using AIDungeonPrompts.Application.Commands.CreateUser;
using AIDungeonPrompts.Application.Commands.UpdateUser;
using AIDungeonPrompts.Application.Exceptions;
using AIDungeonPrompts.Application.Queries.LogIn;
using AIDungeonPrompts.Web.Extensions;
using AIDungeonPrompts.Web.Models.User;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AIDungeonPrompts.Web.Controllers
{
	public class UserController : Controller
	{
		private readonly ICurrentUserService _currentUserService;
		private readonly IMediator _mediator;

		public UserController(IMediator mediator, ICurrentUserService currentUserService)
		{
			_mediator = mediator;
			_currentUserService = currentUserService;
		}

		public IActionResult LogIn()
		{
			return View(new LogInModel());
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> LogIn(string? honey, LogInModel model)
		{
			if (!string.IsNullOrWhiteSpace(honey) || !ModelState.IsValid)
			{
				return View(model);
			}

			try
			{
				var user = await _mediator.Send(model.LogInQuery);
				await HttpContext.SignInUserAsync(user.Id);
			}
			catch (LoginFailedException)
			{
				ModelState.AddModelError(string.Empty, "Username or Password was incorrect");
				return View(model);
			}

			if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
			{
				return Redirect(model.ReturnUrl);
			}
			return RedirectToAction("Index", "Home");
		}

		public IActionResult Register(string returnUrl)
		{
			var model = new RegisterUserModel();
			if (_currentUserService.TryGetCurrentUser(out var user))
			{
				model.Username = user!.Username;
				model.ReturnUrl = returnUrl;
			}
			return View(model);
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(string? honey, RegisterUserModel model)
		{
			if (!string.IsNullOrWhiteSpace(honey))
			{
				return View(model);
			}

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			try
			{
				if (_currentUserService.TryGetCurrentUser(out var user))
				{
					await _mediator.Send(new UpdateUserCommand
					{
						Username = model.Username,
						Password = model.Password,
						Id = user!.Id
					});
				}
				else
				{
					var userId = await _mediator.Send(new CreateUserCommand
					{
						Username = model.Username,
						Password = model.Password
					});
					await HttpContext.SignInUserAsync(userId);
				}
			}
			catch (UsernameNotUniqueException _)
			{
				ModelState.AddModelError(nameof(model.Username), "Username already exists");
				return View(model);
			}

			if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
			{
				return Redirect(model.ReturnUrl);
			}
			return RedirectToAction("Index");
		}
	}
}
