using AIDungeonPrompts.Application.Abstractions.Identity;
using AIDungeonPrompts.Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AIDungeonPrompts.Infrastructure
{
	public static class InfrastructureInjectionExtensions
	{
		public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services)
		{
			return services.AddScoped<ICurrentUserService, CurrentUserService>();
		}
	}
}
