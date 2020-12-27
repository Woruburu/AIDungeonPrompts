using AIDungeonPrompts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIDungeonPrompts.Persistence.Configurations
{
	public class ApplicationLogConfiguration : IEntityTypeConfiguration<ApplicationLog>
	{
		public void Configure(EntityTypeBuilder<ApplicationLog> builder)
		{
			builder.HasKey(e => e.Id);
			builder.Property(e => e.LogEvent).HasColumnType("jsonb");
			builder.Property(e => e.Properties).HasColumnType("jsonb");
		}
	}
}
