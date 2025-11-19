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
		public DbSet<TaskItem> Task { get; set; }
	}
}
