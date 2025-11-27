using DATT.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DATT.API.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}
		public DbSet<User> Users { get; set; }
		public DbSet<TaskItem> Tasks { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<User>()
			.HasData(
				new User
				{
					Id = 1,
					Username = "Admin",
					Email = "admin@gmail.com",
					PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123"),
					Role = "ADMIN"
				}
			);
		}
	}
}
