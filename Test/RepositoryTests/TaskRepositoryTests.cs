using DATT.API.Data;
using DATT.API.Models;
using DATT.API.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.RepositoryTests
{
	public class TaskRepositoryTests
	{
		private AppDbContext GetDbContext()
		{
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;

			return new AppDbContext(options);
		}

		[Fact]
		public async Task AddAsync_Should_Add_Task()
		{
			var context = GetDbContext();
			var repo = new TaskRepository(context);

			var task = new TaskItem
			{
				Title = "Test",
				Description = "Demo",
				UserId = 1
			};

			await repo.AddAsync(task);

			context.Tasks.Count().Should().Be(1);
		}

		[Fact]
		public async Task GetAllTasksAsync_Should_Return_All()
		{
			var context = GetDbContext();

			// ⭐ Thêm Users để tránh lỗi FK
			context.Users.AddRange(
				new User { Id = 1, Username = "A", Email = "a@gmail.com", PasswordHash = "x", Role = "USER" },
				new User { Id = 2, Username = "B", Email = "b@gmail.com", PasswordHash = "x", Role = "USER" }
			);
			await context.SaveChangesAsync();

			context.Tasks.AddRange(
				new TaskItem { Title = "A", UserId = 1 },
				new TaskItem { Title = "B", UserId = 2 }
			);

			await context.SaveChangesAsync();

			var repo = new TaskRepository(context);

			var result = await repo.GetAllTasksAsync();

			result.Count().Should().Be(2);
		}




		[Fact]
		public async Task GetTasksByUserIdAsync_Should_Filter_By_User()
		{
			// Arrange
			var context = GetDbContext();
			var repo = new TaskRepository(context);

			int userId = 1;

			// BẮT BUỘC PHẢI THÊM USER TRƯỚC
			context.Users.AddRange(
				new User { Id = 1, Username = "A", Email = "a@gmail.com", PasswordHash = "x", Role = "USER" },
				new User { Id = 2, Username = "B", Email = "b@gmail.com", PasswordHash = "x", Role = "USER" }
			);
			await context.SaveChangesAsync();

			// ⭐ Sau đó mới thêm Task
			context.Tasks.AddRange(
				new TaskItem { Id = 1, Title = "A", UserId = userId },
				new TaskItem { Id = 2, Title = "B", UserId = userId },
				new TaskItem { Id = 3, Title = "C", UserId = 2 } // user khác
			);

			await context.SaveChangesAsync();

			// Act
			var result = await repo.GetTasksByUserIdAsync(userId);

			// Assert
			result.Count().Should().Be(2);
		}





		[Fact]
		public async Task DeleteAsync_Should_Remove_Task()
		{
			var context = GetDbContext();
			var repo = new TaskRepository(context);

			var task = new TaskItem { Id = 10, Title = "X", UserId = 1 };
			context.Tasks.Add(task);
			await context.SaveChangesAsync();

			await repo.DeleteAsync(10);

			context.Tasks.Count().Should().Be(0);
		}
	}
}
