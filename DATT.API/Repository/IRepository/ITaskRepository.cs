using DATT.API.Models;
using DATT.API.Repositories;

namespace DATT.API.Repository.IRepository
{
	public interface ITaskRepository : IGenericRepository<TaskItem>
	{
		Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId);
		Task<TaskItem?> GetTaskByIdAsync(int id, int userId);

		Task<IEnumerable<TaskItem>> GetAllTasksAsync();

		Task<(IEnumerable<TaskItem> Items, int TotalItems)> QueryAsync(
		int userId,
		int page,
		int pageSize,
		string status,
		string sortBy,
		string sortOrder);
	}
}
