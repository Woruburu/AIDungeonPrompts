using System;
using System.IO;
using System.Linq;
using AIDungeonPrompts.Application;
using AIDungeonPrompts.Backup.Persistence;
using AIDungeonPrompts.Domain;
using AIDungeonPrompts.Infrastructure;
using AIDungeonPrompts.Infrastructure.Identity;
using AIDungeonPrompts.Persistence;
using AIDungeonPrompts.Persistence.DbContexts;
using AIDungeonPrompts.Web.Constants;
using AIDungeonPrompts.Web.HostedServices;
using AIDungeonPrompts.Web.Middleware;
using AIDungeonPrompts.Web.ModelMetadataDetailsProviders;
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
using Microsoft.AspNetCore.StaticFiles;
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

		public Startup(IConfiguration configuration, IWebHostEnvironment environment)
		{
			Configuration = configuration;
			Environment = environment;
		}

		public IConfiguration Configuration { get; }
		public IWebHostEnvironment Environment { get; }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseForwardedHeaders();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
			app.UseXContentTypeOptions();
			app.UseXfo(options => options.Deny());
			app.UseHttpsRedirection();
			app.UseXXssProtection(options => options.EnabledWithBlockMode());
			app.UseReferrerPolicy(opts => opts.NoReferrer());

			app.UseStatusCodePages();

			app.UseCookiePolicy(new CookiePolicyOptions
			{
				HttpOnly = HttpOnlyPolicy.Always,
				Secure = CookieSecurePolicy.Always,
				MinimumSameSitePolicy = SameSiteMode.Strict,
			});

			app.UseCorrelationId();
			app.UseSerilogRequestLogging();

			var provider = new FileExtensionContentTypeProvider();
			provider.Mappings[".db"] = "application/octet-stream";
			app.UseStaticFiles(new StaticFileOptions()
			{
				ContentTypeProvider = provider,
				OnPrepareResponse = (context) =>
				{
					var headers = context.Context.Response.GetTypedHeaders();
					headers.CacheControl = new CacheControlHeaderValue()
					{
						MaxAge = TimeSpan.FromDays(1)
					};
				}
			});

			app.UseRouting();

			app.UseCors();
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
			services.AddCors(options =>
			{
				options.AddPolicy("AiDungeon",
					builder =>
					{
						builder
							.WithOrigins("https://play.aidungeon.io")
							.WithMethods("GET");
					});
			});
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
				.AddApplicationLayer()
				.AddPersistenceLayer(Configuration.GetConnectionString(DatabaseConnectionName))
				.AddBackupPersistenceLayer(BackupDatabaseConnectionName())
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
					builder.ModelMetadataDetailsProviders.Add(new DoNotConvertEmptyStringToNullMetadataDetailsProvider());
				})
				.AddFluentValidation(fv =>
				{
					fv.ImplicitlyValidateChildProperties = true;
					fv.RegisterValidatorsFromAssemblies(new[]
					{
						typeof(ApplicationLayer),
						typeof(Startup)
					}.Select(t => t.Assembly).ToArray());
				});

			services.AddAuthorization(options =>
			{
				options.AddPolicy(
					PolicyValueConstants.EditorsOnly,
					policy => policy.RequireClaim(ClaimValueConstants.CanEdit, true.ToString())
				);
			});

			services.AddHostedService<DatabaseMigrationHostedService>();
			services.AddHostedService<DatabaseBackupHostedService>();
			services.AddHostedService<DatabaseBackupCronJob>();
			services.AddHostedService<ApplicationLogCleanerCronJob>();
			services.AddHostedService<ReportCleanerCronJob>();
		}

		private string BackupDatabaseConnectionName() => $"Data Source={Path.Combine(Environment.WebRootPath, "backup.db")};";
	}
}
