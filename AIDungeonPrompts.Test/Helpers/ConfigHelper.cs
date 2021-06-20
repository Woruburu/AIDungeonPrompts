using Microsoft.Extensions.Configuration;

namespace AIDungeonPrompts.Test.Helpers
{
	public static class ConfigHelper
	{
		public static IConfiguration GetConfiguration() =>
			new ConfigurationBuilder()
				.AddJsonFile("appsettings.Test.json")
				.AddEnvironmentVariables()
				.Build();
	}
}
