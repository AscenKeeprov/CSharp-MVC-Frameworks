using FDMC.Web.Data.EntityConfiguration;
using FDMC.Web.Entities;
using Microsoft.EntityFrameworkCore;

namespace FDMC.Web.Data
{
    public class FDMCDbContext : DbContext
    {
	public virtual DbSet<Cat> Cats { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
	    optionsBuilder
		.UseSqlServer("Server=.\\SQLEXPRESS;Database=FDMCDB;Trusted_Connection=True;")
		.UseLazyLoadingProxies(true);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
	    base.OnModelCreating(modelBuilder);
	    modelBuilder.ApplyConfiguration(new CatEntityConfiguration());
	}
    }
}
