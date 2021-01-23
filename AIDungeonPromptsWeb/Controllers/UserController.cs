using System.Threading;
using System.Threading.Tasks;
using AIDungeonPrompts.Application.Abstractions.Identity;
using AIDungeonPrompts.Application.Commands.CreateUser;
using AIDungeonPrompts.Application.Commands.UpdateUser;
using AIDungeonPrompts.Application.Exceptions;
using AIDungeonPrompts.Application.Queries.GetUser;
using AIDungeonPrompts.Application.Queries.LogIn;
using AIDungeonPrompts.Application.Queries.SearchPrompts;
using AIDungeonPrompts.Web.Extensions;
using AIDungeonPrompts.Web.Models.User;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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

		[Authorize, HttpGet("[controller]/[action]")]

		public IActionResult Edit()
		{
			if (!_currentUserService.TryGetCurrentUser(out var user))
			{
				return NotFound();
			}

			return View(new EditUserModel
			{
				Username = user!.Username
			});
		}

		[Authorize, HttpPost("[controller]/[action]"), ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(EditUserModel model, CancellationToken cancellationToken)
		{
			if (!_currentUserService.TryGetCurrentUser(out var user))
			{
				return NotFound();
			}

			if (!string.Equals(model.Password, model.PasswordConfirm))
			{
				ModelState.AddModelError(nameof(model.PasswordConfirm), "Passwords do not match");
			}

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			try
			{
				await _mediator.Send(new UpdateUserCommand
				{
					Username = model.Username,
					Password = model.Password,
					Id = user!.Id
				}, cancellationToken);
			}
			catch (UsernameNotUniqueException)
			{
				ModelState.AddModelError(nameof(model.Username), "Username already exists");
				return View(model);
			}

			return RedirectToAction("Index");
		}

		[Authorize, HttpGet("[controller]")]
		public async Task<IActionResult> Index(int? page, CancellationToken cancellationToken)
		{
			if (!_currentUserService.TryGetCurrentUser(out var user))
			{
				return NotFound();
			}

			var result = await _mediator.Send(new SearchPromptsQuery
			{
				UserId = user!.Id,
				Page = page ?? 1,
				PageSize = 6,
				IncludeDrafts = true
			}, cancellationToken);

			return View(new IndexUserModel
			{
				Username = user!.Username,
				UserPrompts = result,
				UserRoles = user!.Role,
				Page = page,
				IsTransient = user!.IsTransient
			});
		}

		public IActionResult LogIn(string returnUrl)
		{
			return View(new LogInModel { ReturnUrl = returnUrl });
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> LogIn(LogInModel model, CancellationToken cancellationToken)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			try
			{
				var user = await _mediator.Send(new LogInQuery(model.Username, model.Password), cancellationToken);
				await HttpContext.SignInUserAsync(user);
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

		public IActionResult LogOut()
		{
			HttpContext.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

		public IActionResult Register(string returnUrl)
		{
			var model = new RegisterUserModel
			{
				ReturnUrl = returnUrl
			};
			return View(model);
		}

		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterUserModel model, CancellationToken cancellationToken)
		{
			if (!string.Equals(model.Password, model.PasswordConfirm))
			{
				ModelState.AddModelError(nameof(model.PasswordConfirm), "Passwords do not match");
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
					}, cancellationToken);
				}
				else
				{
					var userId = await _mediator.Send(new CreateUserCommand
					{
						Username = model.Username,
						Password = model.Password
					}, cancellationToken);
					user = await _mediator.Send(new GetUserQuery(userId), cancellationToken);
					await HttpContext.SignInUserAsync(user);
				}
			}
			catch (UsernameNotUniqueException)
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
