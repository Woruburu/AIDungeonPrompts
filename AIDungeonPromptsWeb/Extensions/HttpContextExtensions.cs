using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace AIDungeonPrompts.Web.Extensions
{
	public static class HttpContextExtensions
	{
		public static Task SignInUserAsync(this HttpContext context, int id)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, id.ToString())
			};

			var claimsIdentity = new ClaimsIdentity(
				claims, CookieAuthenticationDefaults.AuthenticationScheme);

			var authProperties = new AuthenticationProperties
			{
				AllowRefresh = true,
				ExpiresUtc = DateTimeOffset.UtcNow.AddDays(365),
				IsPersistent = true,
				IssuedUtc = DateTimeOffset.UtcNow,
			};

			return context.SignInAsync(
				CookieAuthenticationDefaults.AuthenticationScheme,
				new ClaimsPrincipal(claimsIdentity),
				authProperties
			);
		}
	}
}
