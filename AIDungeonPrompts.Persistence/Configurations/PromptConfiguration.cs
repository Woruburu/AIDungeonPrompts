using AIDungeonPrompts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIDungeonPrompts.Persistence.Configurations
{
	public class PromptConfiguration : IEntityTypeConfiguration<Prompt>
	{
		public void Configure(EntityTypeBuilder<Prompt> builder)
		{
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Title).IsRequired();
			builder.Property(e => e.PromptContent).IsRequired();
			builder.Property(e => e.ScriptZip).HasMaxLength(5000000);
			builder.Property(e => e.NovelAiScenario).HasColumnType("jsonb");
			builder.HasIndex(e => e.Title);
			builder.HasOne(e => e.Owner)
				.WithMany(e => e.Prompts)
				.HasForeignKey(e => e.OwnerId);
		}
	}
}
