using System;
using System.Linq;
using AIDungeonPrompts.Application;
using AIDungeonPrompts.Domain;
using AIDungeonPrompts.Infrastructure;
using AIDungeonPrompts.Infrastructure.Identity;
using AIDungeonPrompts.Persistence;
using AIDungeonPrompts.Persistence.DbContexts;
using CorrelationId;
using CorrelationId.DependencyInjection;
using FluentValidation.AspNetCore;
using MediatR;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Serilog;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

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
				app.UseHttpsRedirection();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseStatusCodePages();

			app.UseCookiePolicy(new CookiePolicyOptions
			{
				HttpOnly = HttpOnlyPolicy.Always,
				Secure = CookieSecurePolicy.Always,
				MinimumSameSitePolicy = SameSiteMode.Strict
			});

			context.Database.Migrate();

			app.UseStaticFiles(new StaticFileOptions()
			{
				OnPrepareResponse = (context) =>
				{
					var headers = context.Context.Response.GetTypedHeaders();
					headers.CacheControl = new CacheControlHeaderValue()
					{
						MaxAge = TimeSpan.FromDays(1)
					};
				}
			});

			app.UseHttpsRedirection();

			app.UseCorrelationId();

			app.UseSerilogRequestLogging();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseMiddleware<CurrentUserMiddleware>();

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
			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie();

			services.AddDataProtection()
				.PersistKeysToDbContext<AIDungeonPromptsDbContext>();

			services
				.AddPersistenceLayer(Configuration.GetConnectionString(DatabaseConnectionName))
				.AddInfrastructureLayer()
				.AddHttpContextAccessor()
				.AddDefaultCorrelationId()
				.AddDistributedMemoryCache()
				.AddMediatR(new[] { typeof(DomainLayer), typeof(ApplicationLayer) }.Select(t => t.Assembly).ToArray())
				.AddFluentValidation(new[] { typeof(ApplicationLayer) }.Select(t => t.Assembly).ToArray())
				.AddRouting(builder =>
				{
					builder.LowercaseUrls = true;
					builder.LowercaseQueryStrings = true;
				})
				.AddControllersWithViews()
				.AddFluentValidation(fv =>
					fv.RegisterValidatorsFromAssemblies(new[]
					{
						typeof(ApplicationLayer),
						typeof(Startup)
					}.Select(t => t.Assembly).ToArray())
				);
			// See: https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-storage-providers?view=aspnetcore-5.0&tabs=visual-studio#entity-framework-core
		}
	}
}
