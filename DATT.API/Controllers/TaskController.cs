using DATT.API.Data;
using DATT.API.DTOs.Task;
using DATT.API.Extensions;
using DATT.API.Models;
using DATT.API.Models.Responses;
using DATT.API.Repositories;
using DATT.API.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATT.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class TaskController : ControllerBase
	{
		private readonly ITaskRepository _taskRepo;
		private readonly ILogger<TaskController> _logger;

		public TaskController(ITaskRepository taskRepo, ILogger<TaskController> logger)
		{
			_taskRepo = taskRepo;
			_logger = logger;
		}

		// ADMIN: lấy toàn bộ task
		[Authorize(Policy = "AdminOnly")]
		[HttpGet("admin/all")]
		public async Task<IActionResult> GetAllForAdmin()
		{
			_logger.LogInformation("Admin đang lấy toàn bộ task");

			var tasks = await _taskRepo.GetAllTasksAsync();

			return Ok(new ApiResponse<object>
			{
				StatusCode = 200,
				Data = tasks,
				Message = "Lấy toàn bộ task thành công"
			});
		}

		// USER: lấy task của chính mình
		[HttpGet]
		public async Task<IActionResult> GetMyTasks()
		{
			int userId = User.GetUserId();
			_logger.LogInformation("User {UserId} đang lấy danh sách task", userId);

			var tasks = await _taskRepo.GetTasksByUserIdAsync(userId);

			return Ok(new ApiResponse<object>
			{
				StatusCode = 200,
				Data = tasks,
				Message = "Lấy danh sách task thành công"
			});
		}

		// USER: lấy 1 task của chính mình
		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			int userId = User.GetUserId();
			var task = await _taskRepo.GetTaskByIdAsync(id, userId);

			if (task == null)
				throw new AppException("Không có quyền xem task này", 403);

			return Ok(new ApiResponse<object>
			{
				StatusCode = 200,
				Data = task,
				Message = "Lấy task thành công"
			});
		}


		[HttpGet("query")]
		public async Task<IActionResult> Query(
		int page = 1,
		int pageSize = 5,
		string status = "all",
		string sortBy = "createdAt",
		string sortOrder = "asc")
		{
			int userId = User.GetUserId();

			var (items, totalItems) = await _taskRepo.QueryAsync(
				userId, page, pageSize, status, sortBy, sortOrder);

			return Ok(new ApiResponse<object>
			{
				StatusCode = 200,
				Message = "Lấy danh sách task thành công",
				Data = items,
				Meta = new
				{
					page,
					pageSize,
					totalItems,
					totalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
				}
			});
		}


		// USER: tạo task
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] TaskCreateDto dto)
		{
			int userId = User.GetUserId();

			var task = new TaskItem
			{
				Title = dto.Title,
				Description = dto.Description,
				IsCompleted = dto.IsCompleted,
				Deadline = dto.Deadline,
				UserId = userId
			};


			await _taskRepo.AddAsync(task);

			return Ok(new ApiResponse<object>
			{
				StatusCode = 200,
				Data = task,
				Message = "Tạo task thành công"
			});
		}

		// USER: update task
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] TaskUpdateDto dto)
		{
			int userId = User.GetUserId();
			var task = await _taskRepo.GetTaskByIdAsync(id, userId);

			if (task == null)
				throw new AppException("Không có quyền sửa task này", 403);

			task.Title = dto.Title;
			task.Description = dto.Description;
			task.IsCompleted = dto.IsCompleted;
			task.Deadline = dto.Deadline;

			await _taskRepo.UpdateAsync(task);

			return Ok(new ApiResponse<object>
			{
				StatusCode = 200,
				Data = task,
				Message = "Cập nhật task thành công"
			});
		}

		// USER: delete task
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			int userId = User.GetUserId();
			var task = await _taskRepo.GetTaskByIdAsync(id, userId);

			if (task == null)
				throw new AppException("Không có quyền xoá task này", 403);

			await _taskRepo.DeleteAsync(id);

			return Ok(new ApiResponse<object>
			{
				StatusCode = 200,
				Data = null,
				Message = "Xoá task thành công"
			});
		}
	}
}
