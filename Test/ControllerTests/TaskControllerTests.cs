using DATT.API.Controllers;
using DATT.API.DTOs.Task;
using DATT.API.Extensions;
using DATT.API.Models;
using DATT.API.Models.Responses;
using DATT.API.Repository.IRepository;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Test.ControllerTests
{
	public class TaskControllerTests
	{
		private TaskController GetController(Mock<ITaskRepository> repoMock)
		{
			var loggerMock = new Mock<ILogger<TaskController>>();
			var controller = new TaskController(repoMock.Object, loggerMock.Object);

			// Fake user ID = 5
			var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
			{
				new Claim("id", "5")
			}, "mock"));

			controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = user }
			};

			return controller;
		}

		[Fact]
		public async Task GetMyTasks_Should_Return_User_Tasks()
		{
			var repoMock = new Mock<ITaskRepository>();

			repoMock
				.Setup(r => r.GetTasksByUserIdAsync(5))
				.ReturnsAsync(new List<TaskItem>
				{
					new TaskItem { Id = 1, Title = "A", UserId = 5 },
					new TaskItem { Id = 2, Title = "B", UserId = 5 }
				});

			var controller = GetController(repoMock);

			var result = await controller.GetMyTasks() as OkObjectResult;
			var response = result!.Value as ApiResponse<object>;

			((IEnumerable<TaskItem>)response!.Data!).Count().Should().Be(2);
		}

		[Fact]
		public async Task Create_Should_Add_New_Task()
		{
			var repoMock = new Mock<ITaskRepository>();

			var controller = GetController(repoMock);

			var dto = new TaskCreateDto
			{
				Title = "New",
				Description = "Demo",
				IsCompleted = false
			};

			var result = await controller.Create(dto) as OkObjectResult;

			result.Should().NotBeNull();
			repoMock.Verify(r => r.AddAsync(It.IsAny<TaskItem>()), Times.Once);
		}

		[Fact]
		public async Task Update_Should_Return_403_When_Not_Owner()
		{
			var repoMock = new Mock<ITaskRepository>();

			repoMock
				.Setup(r => r.GetTaskByIdAsync(1, 5))
				.ReturnsAsync((TaskItem?)null);

			var controller = GetController(repoMock);

			var dto = new TaskUpdateDto
			{
				Title = "X",
				Description = "Y",
				IsCompleted = false
			};

			Func<Task> act = async () => await controller.Update(1, dto);

			await act.Should().ThrowAsync<AppException>()
				.Where(e => e.StatusCode == 403);
		}

		[Fact]
		public async Task Delete_Should_Delete_Task()
		{
			var repoMock = new Mock<ITaskRepository>();

			repoMock
				.Setup(r => r.GetTaskByIdAsync(1, 5))
				.ReturnsAsync(new TaskItem { Id = 1, UserId = 5 });

			var controller = GetController(repoMock);

			var result = await controller.Delete(1) as OkObjectResult;

			repoMock.Verify(r => r.DeleteAsync(1), Times.Once);
		}
	}
}
