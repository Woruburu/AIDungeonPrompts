using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AIDungeonPrompts.Application.Behaviours
{
	public class RequestPerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
		where TRequest : notnull
	{
		private readonly ILogger<TRequest> _logger;
		private readonly Stopwatch _timer;

		public RequestPerformanceBehavior(ILogger<TRequest> logger)
		{
			_timer = new Stopwatch();
			_logger = logger;
		}

		public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
			RequestHandlerDelegate<TResponse> next)
		{
			_timer.Start();
			try
			{
				TResponse response = await next();
				_timer.Stop();
				LogTime(request);
				return response;
			}
			catch
			{
				_timer.Stop();
				LogTime(request, true);
				throw;
			}
		}

		private void LogTime(TRequest request, bool errored = false)
		{
			var name = typeof(TRequest).Name;

			if (_timer.ElapsedMilliseconds > 2000)
			{
				_logger.LogWarning(
					$"Request {name} {(errored ? "failed in" : "took")} {_timer.ElapsedMilliseconds} milliseconds which was over longer than 2000 ms with parameters {System.Text.Json.JsonSerializer.Serialize(request)}");
			}
		}
	}
}
