using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AIDungeonPrompts.Application.Behaviours
{
	public class ExceptionLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
		where TRequest : notnull
	{
		private readonly ILogger<TRequest> _logger;

		public ExceptionLoggingBehavior(ILogger<TRequest> logger)
		{
			_logger = logger;
		}

		public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
			RequestHandlerDelegate<TResponse> next)
		{
			try
			{
				return await next();
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"Request {nameof(TRequest)} threw an exception.");
				throw;
			}
		}
	}
}
