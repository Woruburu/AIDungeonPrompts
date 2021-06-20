using AIDungeonPrompts.Application.Behaviours;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AIDungeonPrompts.Application
{
	public static class ApplicationInjectionExtensions
	{
		public static IServiceCollection AddApplicationLayer(this IServiceCollection services) =>
			services
				.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehavior<,>))
				.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionLoggingBehavior<,>));
	}
}
