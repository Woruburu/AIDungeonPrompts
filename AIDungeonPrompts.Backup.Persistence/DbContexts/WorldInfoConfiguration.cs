using AIDungeonPrompts.Backup.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIDungeonPrompts.Backup.Persistence.Configurations
{
	public class WorldInfoConfiguration : IEntityTypeConfiguration<BackupWorldInfo>
	{
		public void Configure(EntityTypeBuilder<BackupWorldInfo> builder)
		{
			builder.HasKey(e => e.Id);
			builder.HasIndex(e => e.CorrelationId);
			builder.HasOne(e => e.Prompt)
				.WithMany(e => e.WorldInfos)
				.HasForeignKey(e => e.PromptId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
