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
			builder.HasIndex(e => e.Title);
			builder.HasOne(e => e.Owner)
				.WithMany(e => e.Prompts)
				.HasForeignKey(e => e.OwnerId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
