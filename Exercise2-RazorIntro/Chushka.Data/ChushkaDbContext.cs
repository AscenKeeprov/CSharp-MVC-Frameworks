using Chushka.Data.EntityConfiguration;
using Chushka.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chushka.Data
{
    public class ChushkaDbContext : IdentityDbContext<User, IdentityRole, string>
    {
	public ChushkaDbContext() { }

	public ChushkaDbContext(DbContextOptions<ChushkaDbContext> options) : base(options) { }

	public virtual DbSet<Product> Products { get; set; }
	public virtual DbSet<Order> Orders { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
	    optionsBuilder
		.UseSqlServer("Server=.\\SQLEXPRESS;Database=ChushkaMVCDB;Integrated Security=True;MultipleActiveResultSets=true;")
		.UseLazyLoadingProxies(true);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
	    base.OnModelCreating(modelBuilder);
	    modelBuilder.ApplyConfiguration(new OrderEntityConfiguration());
	    modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
	    modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
	}
    }
}
