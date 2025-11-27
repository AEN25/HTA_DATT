 using DATT.API.Data;
using DATT.API.Models;
using DATT.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DATT.API.Repository
{
	public class UserRepository : GenericRepository<User>
	{
		public UserRepository(AppDbContext context) : base(context)
		{
		}
		public override async Task<IEnumerable<User>> GetAllAsync()
		{
			return await _context.Users
				.Include(u => u.Tasks)      
				.ToListAsync();
		}

		public override async Task<User?> GetByIdAsync(int id)
		{
			return await _context.Users
				.Include(u => u.Tasks)      
				.FirstOrDefaultAsync(u => u.Id == id);
		}
	}
}
