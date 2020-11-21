using System;
using AIDungeonPrompts.Application.Abstractions.DbContexts;
using AIDungeonPrompts.Domain.Entities;
using AIDungeonPrompts.Persistence.DbContexts;
using Audit.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AIDungeonPrompts.Persistence
{
	public static class PersistenceInjectionExtensions
	{
		public static IServiceCollection AddPersistenceLayer(this IServiceCollection services,
			string databaseConnection)
		{
			services
				.AddDbContext<AIDungeonPromptsDbContext>(options => options.UseNpgsql(databaseConnection, builder =>
				{
					// See: https://docs.microsoft.com/en-us/ef/core/querying/single-split-queries
					builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
				}))
				.AddScoped<IAIDungeonPromptsDbContext>(provider => provider.GetService<AIDungeonPromptsDbContext>()!);

			_ = Configuration.Setup()
				.UseEntityFramework(ef => ef
					.AuditTypeExplicitMapper(m => m
						.Map<Prompt, AuditPrompt>((prompt, auditEntity) =>
						{
							auditEntity.PromptId = prompt.Id;
						})
						.Map<PromptTag, AuditPrompt>((tag, auditEntity) =>
						{
							auditEntity.PromptId = tag.PromptId;
						})
						.Map<WorldInfo, AuditPrompt>((wi, auditEntity) =>
						{
							auditEntity.PromptId = wi.PromptId;
						})
						.AuditEntityAction<AuditPrompt>((evt, entry, auditEntity) =>
						{
							auditEntity.AuditDate = DateTime.UtcNow;
							auditEntity.Entry = entry.ToJson();
						})
					).IgnoreMatchedProperties(true)
				);

			return services;
		}
	}
}
