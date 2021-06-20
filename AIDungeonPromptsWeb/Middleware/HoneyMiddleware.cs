using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace AIDungeonPrompts.Web.Middleware
{
	public class HoneyMiddleware
	{
		private readonly ILogger<HoneyMiddleware> _logger;
		private readonly RequestDelegate _next;

		public HoneyMiddleware(RequestDelegate next, ILogger<HoneyMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			if (!context.Request.HasFormContentType)
			{
				await _next(context);
				return;
			}

			KeyValuePair<string, StringValues> result = context
				.Request
				.Form
				.FirstOrDefault(e => string.Equals("honey", e.Key, StringComparison.OrdinalIgnoreCase));

			if (result.Value.ToString() != string.Empty)
			{
				context.Response.StatusCode = 400;
				return;
			}

			await _next(context);
		}
	}
}
