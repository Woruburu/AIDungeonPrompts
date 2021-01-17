using AIDungeonPrompts.Domain.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIDungeonPrompts.Persistence.Configurations
{
	public class NonDraftPromptConfiguration : IEntityTypeConfiguration<NonDraftPrompt>
	{
		public void Configure(EntityTypeBuilder<NonDraftPrompt> builder)
		{
			builder.HasNoKey();
			builder.ToView("NonDraftPrompts");
		}
	}
}
