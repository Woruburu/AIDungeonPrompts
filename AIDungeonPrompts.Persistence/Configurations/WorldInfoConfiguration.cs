using AIDungeonPrompts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIDungeonPrompts.Persistence.Configurations
{
	public class WorldInfoConfiguration : IEntityTypeConfiguration<WorldInfo>
	{
		public void Configure(EntityTypeBuilder<WorldInfo> builder)
		{
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Keys).IsRequired();
			builder.Property(e => e.Entry).IsRequired();
			builder.HasOne(e => e.Prompt)
				.WithMany()
				.HasForeignKey(e => e.PromptId)
				.IsRequired();
		}
	}
}
