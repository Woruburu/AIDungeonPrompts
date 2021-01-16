using System.Data.SQLite;
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
			services.AddDbContext<BackupDbContext>(options =>
			{
				var conn = new SQLiteConnection(databaseConnection);
				conn.Open();

				var command = conn.CreateCommand();
				command.CommandText = "PRAGMA journal_mode = TRUNCATE";
				command.ExecuteNonQuery();

				options.UseSqlite(conn);
			});
			return services;
		}
	}
}
