using System.Linq;
using AIDungeonPrompts.Application;
using AIDungeonPrompts.Domain;
using AIDungeonPrompts.Persistence;
using AIDungeonPrompts.Persistence.DbContexts;
using CorrelationId;
using CorrelationId.DependencyInjection;
using MediatR;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace AIDungeonPrompts.Web
{
	public class Startup
	{
		private const string DatabaseConnectionName = "AIDungeonPrompt";

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AIDungeonPromptsDbContext context)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			context.Database.Migrate();

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseHttpsRedirection();

			app.UseCorrelationId();

			app.UseSerilogRequestLogging();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services
				.AddPersistenceLayer(Configuration.GetConnectionString(DatabaseConnectionName))
				.AddHttpContextAccessor()
				.AddDefaultCorrelationId()
				.AddDistributedMemoryCache()
				.AddMediatR(new[] { typeof(DomainLayer), typeof(ApplicationLayer) }.Select(t => t.Assembly).ToArray())
				.AddFluentValidation(new[] { typeof(ApplicationLayer) }.Select(t => t.Assembly).ToArray())
				.AddRouting(builder => builder.LowercaseUrls = true)
				.AddControllersWithViews();
			// See: https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-storage-providers?view=aspnetcore-5.0&tabs=visual-studio#entity-framework-core
			services.AddDataProtection()
				.PersistKeysToDbContext<AIDungeonPromptsDbContext>();
		}
	}
}
