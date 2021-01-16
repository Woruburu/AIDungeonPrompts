using AIDungeonPrompts.Backup.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AIDungeonPrompts.Backup.Persistence
{
	public static class BackupPersistenceInjectionExtensions
	{
		public static IServiceCollection AddBackupPersistenceLayer(this IServiceCollection services,
			string databaseConnection)
		{
			services.AddDbContext<BackupDbContext>(options => options.UseSqlite(databaseConnection));
			return services;
		}
	}
}
