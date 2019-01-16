using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace PertensaCo.Data.Extensions
{
	public static class DbContextExtensions
	{
		public static async Task CreateDatabaseAsync(this DbContext dbContext)
		{
			if (dbContext.Database.GetMigrations().Any())
			{
				await dbContext.Database.MigrateAsync();
			}
			else await dbContext.Database.EnsureCreatedAsync();
		}

		public static async Task<bool> DatabaseExistsAsync(this DbContext dbContext)
		{
			var dbCreatorService = dbContext.Database
				.GetService<IRelationalDatabaseCreator>();
			if (dbCreatorService == null) return false;
			return await dbCreatorService.ExistsAsync();
		}

		public static Type GetEntityPrimaryKeyType(this DbContext dbContext, Type type)
		{
			var entityType = dbContext.Model.FindEntityType(type);
			var primaryKey = entityType.FindPrimaryKey();
			Type primaryKeyType = primaryKey.Properties[0].ClrType;
			return primaryKeyType;
		}
	}
}
