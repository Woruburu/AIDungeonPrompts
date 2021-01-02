using System;
using System.Linq;
using AIDungeonPrompts.Application;
using AIDungeonPrompts.Domain;
using AIDungeonPrompts.Infrastructure;
using AIDungeonPrompts.Infrastructure.Identity;
using AIDungeonPrompts.Persistence;
using AIDungeonPrompts.Persistence.DbContexts;
using AIDungeonPrompts.Web.Constants;
using AIDungeonPrompts.Web.HostedServices;
using AIDungeonPrompts.Web.Middleware;
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
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using NWebsec.AspNetCore.Mvc.Csp;
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
			app.UseForwardedHeaders();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseHttpsRedirection();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.Use(async (context, next) =>
			{
				context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
				await next();
			});

			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();

			app.UseStatusCodePages();

			app.UseCookiePolicy(new CookiePolicyOptions
			{
				HttpOnly = HttpOnlyPolicy.Always,
				Secure = CookieSecurePolicy.Always,
				MinimumSameSitePolicy = SameSiteMode.Strict,
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
			app.UseXXssProtection(options => options.EnabledWithBlockMode());

			app.UseCorrelationId();

			app.UseSerilogRequestLogging();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseMiddleware<CurrentUserMiddleware>();
			app.UseMiddleware<HoneyMiddleware>();

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
			services.Configure<ForwardedHeadersOptions>(options =>
			{
				options.ForwardedHeaders =
					ForwardedHeaders.XForwardedFor |
					ForwardedHeaders.XForwardedProto |
					ForwardedHeaders.XForwardedHost;
			});

			services
				.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(builder =>
				{
					builder.LoginPath = "/user/login";
				});

			// See: https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-storage-providers?view=aspnetcore-5.0&tabs=visual-studio#entity-framework-core
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
				.AddControllersWithViews(builder =>
				{
					builder.Filters.Add(typeof(CspAttribute));
					builder.Filters.Add(new CspDefaultSrcAttribute { Self = true });
					builder.Filters.Add(new CspImgSrcAttribute { Self = true, CustomSources = "data:" });
					builder.Filters.Add(new CspScriptSrcAttribute { Self = true, UnsafeEval = false, UnsafeInline = false });
					builder.Filters.Add(new CspStyleSrcAttribute { Self = true, UnsafeInline = false });
					builder.Filters.Add(new CspObjectSrcAttribute { None = true });
				})
				.AddFluentValidation(fv =>
					fv.RegisterValidatorsFromAssemblies(new[]
					{
						typeof(ApplicationLayer),
						typeof(Startup)
					}.Select(t => t.Assembly).ToArray())
				);

			services.AddAuthorization(options =>
			{
				options.AddPolicy(
					PolicyValueConstants.EditorsOnly,
					policy => policy.RequireClaim(ClaimValueConstants.CanEdit, true.ToString())
				);
			});

			services.AddHostedService<ApplicationLogCleanerHostedService>();
			services.AddHostedService<ReportCleanerHostedService>();
			// This shouldn't ever need to be enabled again
			//services.AddHostedService<NewlineFixerHostedService>();
		}
	}
}
