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

		private static IHostBuilder CreateHostBuilder(string[] args)
		{
			return Host.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((hostingContext, config) =>
				{
					var env = hostingContext.HostingEnvironment;
					config.AddJsonFile($"appSettings.{Environment.MachineName}.json", optional: true,
							reloadOnChange: true)
						.AddJsonFile("serilog.json", optional: false, reloadOnChange: true)
						.AddJsonFile($"serilog.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
						.AddJsonFile($"serilog.{Environment.MachineName}.json", optional: true, reloadOnChange: true);
				})
				.UseSerilog((context, services, configuration) =>
				{
					configuration.ReadFrom.Configuration(context.Configuration);
					configuration.ReadFrom.Services(services);
				})
				.ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
		}
	}
}
