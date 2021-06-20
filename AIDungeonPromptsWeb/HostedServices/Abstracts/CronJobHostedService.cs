using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace AIDungeonPrompts.Web.HostedServices.Abstracts
{
	public abstract class CronJobHostedService : IHostedService, IDisposable
	{
		private readonly CronExpression _expression;
		private readonly ILogger<CronJobHostedService> _logger;
		private readonly TimeZoneInfo _timeZoneInfo;
		private bool _disposedValue;
		private Timer? _timer;

		protected CronJobHostedService(string cronExpression, TimeZoneInfo timeZoneInfo,
			ILogger<CronJobHostedService> logger)
		{
			_expression = CronExpression.Parse(cronExpression);
			_timeZoneInfo = timeZoneInfo;
			_logger = logger;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual Task StartAsync(CancellationToken cancellationToken) => ScheduleJob(cancellationToken);

		public virtual Task StopAsync(CancellationToken cancellationToken)
		{
			_timer?.Stop();
			return Task.CompletedTask;
		}

		public abstract Task DoWork(CancellationToken cancellationToken);

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					_timer?.Dispose();
				}

				_disposedValue = true;
			}
		}

		protected virtual async Task ScheduleJob(CancellationToken cancellationToken)
		{
			DateTimeOffset? next = _expression.GetNextOccurrence(DateTimeOffset.Now, _timeZoneInfo);
			if (next.HasValue)
			{
				TimeSpan delay = next.Value - DateTimeOffset.Now;
				if (delay.TotalMilliseconds <= 0) // prevent non-positive values from being passed into Timer
				{
					await ScheduleJob(cancellationToken);
				}

				_timer = new Timer(delay.TotalMilliseconds);
				_timer.Elapsed += async (sender, args) =>
				{
					_timer.Dispose(); // reset and dispose timer
					_timer = null;

					if (!cancellationToken.IsCancellationRequested)
					{
						try
						{
							await DoWork(cancellationToken);
						}
						catch (Exception e)
						{
							_logger.LogError(e, "Cronjob failed.");
						}
					}

					if (!cancellationToken.IsCancellationRequested)
					{
						await ScheduleJob(cancellationToken); // reschedule next
					}
				};
				_timer.Start();
			}

			await Task.CompletedTask;
		}
	}
}
