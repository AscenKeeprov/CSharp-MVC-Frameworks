using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PertensaCo.Data.Configurations;
using PertensaCo.Entities;

namespace PertensaCo.Data
{
	public class PertensaDbContext : IdentityDbContext<User, Role, string,
	UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
	{
		public PertensaDbContext() { }

		public PertensaDbContext(DbContextOptions<PertensaDbContext> options)
			: base(options) { }

		public virtual DbSet<Alloy> Alloys { get; set; }
		public virtual DbSet<Attachment> Attachments { get; set; }
		public virtual DbSet<Client> Clients { get; set; }
		public virtual DbSet<Employee> Employees { get; set; }
		public virtual DbSet<Material> Warehouse { get; set; }
		public virtual DbSet<Message> Messages { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder
				.UseSqlServer(PertensaDbContextConfiguration.ConnectionString)
				.UseLazyLoadingProxies(true);
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.ApplyConfiguration(new AlloyEntityConfiguration());
			modelBuilder.ApplyConfiguration(new AttachmentEntityConfiguration());
			modelBuilder.ApplyConfiguration(new ClientEntityConfiguration());
			modelBuilder.ApplyConfiguration(new EmployeeEntityConfiguration());
			modelBuilder.ApplyConfiguration(new MaterialEntityConfiguration());
			modelBuilder.ApplyConfiguration(new MessageEntityConfiguration());
			modelBuilder.ApplyConfiguration(new RoleEntityConfiguration());
			modelBuilder.ApplyConfiguration(new RoleClaimEntityConfiguration());
			modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
			modelBuilder.ApplyConfiguration(new UserClaimEntityConfiguration());
			modelBuilder.ApplyConfiguration(new UserLoginEntityConfiguration());
			modelBuilder.ApplyConfiguration(new UserRoleEntityConfiguration());
			modelBuilder.ApplyConfiguration(new UserTokenEntityConfiguration());
		}
	}
}
