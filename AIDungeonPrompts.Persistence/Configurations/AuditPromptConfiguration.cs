using AIDungeonPrompts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIDungeonPrompts.Persistence.Configurations
{
	public class AuditPromptConfiguration : IEntityTypeConfiguration<AuditPrompt>
	{
		public void Configure(EntityTypeBuilder<AuditPrompt> builder)
		{
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Entry).HasColumnType("jsonb");
		}
	}
}
