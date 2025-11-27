using DATT.API.Data;
using DATT.API.Models;
using DATT.API.Repositories;
using DATT.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace DATT.API.Repository
{
	public class TaskRepository: GenericRepository<TaskItem>, ITaskRepository
	{
		public TaskRepository(AppDbContext context) : base(context) { }

		public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
		{
			return await _dbSet
				.Include(t => t.User)
				.ToListAsync();
		}

		public async Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId)
		{
			return await _dbSet
				.Where(t => t.UserId == userId)
				.Include(t => t.User)
				.ToListAsync();
		}

		public async Task<TaskItem?> GetTaskByIdAsync(int id, int userId)
		{
			return await _dbSet
				.Where(t => t.Id == id && t.UserId == userId)
				.Include(t => t.User)
				.FirstOrDefaultAsync();
		}
		public async Task<(IEnumerable<TaskItem>, int)> QueryAsync(
		int userId,
		int page,
		int pageSize,
		string status,
		string sortBy,
		string sortOrder)
		{
			var query = _context.Tasks.Where(t => t.UserId == userId);

			// Filtering
			if (status == "completed")
				query = query.Where(t => t.IsCompleted);

			if (status == "pending")
				query = query.Where(t => !t.IsCompleted);

			// Sorting
			query = sortBy switch
			{
				"deadline" => sortOrder == "asc"
					? query.OrderBy(t => t.Deadline)
					: query.OrderByDescending(t => t.Deadline),

				_ => sortOrder == "asc"
					? query.OrderBy(t => t.CreatedAt)
					: query.OrderByDescending(t => t.CreatedAt)
			};

			int totalItems = await query.CountAsync();

			// Pagination
			var items = await query
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return (items, totalItems);
		}

	}
}
