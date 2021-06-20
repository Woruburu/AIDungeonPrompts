using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace AIDungeonPrompts.Web
{
	public static class Program
	{
		public static int Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.Enrich.FromLogContext()
				.WriteTo.Console()
				.CreateBootstrapLogger();

			try
			{
				Log.Information("Starting web host");
				CreateHostBuilder(args).Build().Run();
				return 0;
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "Host terminated unexpectedly");
				return 1;
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		private static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((hostingContext, config) =>
				{
					IHostEnvironment? env = hostingContext.HostingEnvironment;
					config.AddJsonFile($"appSettings.{Environment.MachineName}.json", true,
							true)
						.AddJsonFile("serilog.json", false, true)
						.AddJsonFile($"serilog.{env.EnvironmentName}.json", true, true)
						.AddJsonFile($"serilog.{Environment.MachineName}.json", true, true);
				})
				.UseSerilog((context, services, configuration) =>
				{
					configuration.ReadFrom.Configuration(context.Configuration);
					configuration.ReadFrom.Services(services);
				})
				.ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
	}
}
