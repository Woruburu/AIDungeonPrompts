using AIDungeonPrompts.Backup.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIDungeonPrompts.Backup.Persistence.Configurations
{
	public class PromptConfiguration : IEntityTypeConfiguration<BackupPrompt>
	{
		public void Configure(EntityTypeBuilder<BackupPrompt> builder)
		{
			builder.HasKey(e => e.Id);
			builder.HasIndex(e => e.CorrelationId);
		}
	}
}
