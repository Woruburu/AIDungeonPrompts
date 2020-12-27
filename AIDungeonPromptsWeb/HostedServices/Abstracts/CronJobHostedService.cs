using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.Hosting;

namespace AIDungeonPrompts.Web.HostedServices.Abstracts
{
	public abstract class CronJobHostedService : IHostedService, IDisposable
	{
		private readonly CronExpression _expression;
		private readonly TimeZoneInfo _timeZoneInfo;
		private bool _disposedValue;
		private System.Timers.Timer? _timer;

		protected CronJobHostedService(string cronExpression, TimeZoneInfo timeZoneInfo)
		{
			_expression = CronExpression.Parse(cronExpression);
			_timeZoneInfo = timeZoneInfo;
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public abstract Task DoWork(CancellationToken cancellationToken);

		public virtual Task StartAsync(CancellationToken cancellationToken)
		{
			return ScheduleJob(cancellationToken);
		}

		public virtual Task StopAsync(CancellationToken cancellationToken)
		{
			_timer?.Stop();
			return Task.CompletedTask;
		}

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
			var next = _expression.GetNextOccurrence(DateTimeOffset.Now, _timeZoneInfo);
			if (next.HasValue)
			{
				var delay = next.Value - DateTimeOffset.Now;
				if (delay.TotalMilliseconds <= 0)   // prevent non-positive values from being passed into Timer
				{
					await ScheduleJob(cancellationToken);
				}
				_timer = new System.Timers.Timer(delay.TotalMilliseconds);
				_timer.Elapsed += async (sender, args) =>
				{
					_timer.Dispose();  // reset and dispose timer
					_timer = null;

					if (!cancellationToken.IsCancellationRequested)
					{
						await DoWork(cancellationToken);
					}

					if (!cancellationToken.IsCancellationRequested)
					{
						await ScheduleJob(cancellationToken);    // reschedule next
					}
				};
				_timer.Start();
			}
			await Task.CompletedTask;
		}
	}
}
