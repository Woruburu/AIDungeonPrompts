using AIDungeonPrompts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIDungeonPrompts.Persistence.Configurations
{
	public class PromptTagConfiguration : IEntityTypeConfiguration<PromptTag>
	{
		public void Configure(EntityTypeBuilder<PromptTag> builder)
		{
			builder.HasKey(e => e.Id);
			builder
				.HasOne(e => e.Prompt)
				.WithMany(e => e.PromptTags)
				.HasForeignKey(e => e.PromptId)
				.IsRequired();
			builder
				.HasOne(e => e.Tag)
				.WithMany(e => e.PromptTags)
				.HasForeignKey(e => e.TagId)
				.IsRequired();
		}
	}
}
