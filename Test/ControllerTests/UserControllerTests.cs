using DATT.API.Controllers;
using DATT.API.Models;
using DATT.API.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.ControllerTests
{
	public class UserControllerTests
	{
		private readonly Mock<IGenericRepository<User>> _repoMock;
		private readonly UserController _controller;

		public UserControllerTests()
		{
			_repoMock = new Mock<IGenericRepository<User>>();
			_controller = new UserController(_repoMock.Object);
		}

		// ========== GET ALL ==========
		[Fact]
		public async Task GetAll_Should_Return_All_Users()
		{
			var fakeUsers = new List<User>
			{
				new User { Id = 1, Username = "A" },
				new User { Id = 2, Username = "B" }
			};

			_repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(fakeUsers);

			var result = await _controller.GetAll() as OkObjectResult;

			result.Should().NotBeNull();
			var users = result.Value as IEnumerable<User>;
			users.Count().Should().Be(2);
		}

		// ========== GET BY ID ==========
		[Fact]
		public async Task Get_Should_Return_User_When_Found()
		{
			var user = new User { Id = 1, Username = "A" };
			_repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

			var result = await _controller.Get(1) as OkObjectResult;

			result.Should().NotBeNull();
			((User)result.Value).Id.Should().Be(1);
		}

		[Fact]
		public async Task Get_Should_Return_NotFound_When_User_Not_Exist()
		{
			_repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

			var result = await _controller.Get(99);

			result.Should().BeOfType<NotFoundResult>();
		}

		// ========== CREATE ==========
		[Fact]
		public async Task Create_Should_Add_User()
		{
			var newUser = new User { Id = 10, Username = "NewUser" };

			_repoMock.Setup(r => r.AddAsync(newUser)).Returns(Task.CompletedTask);

			var result = await _controller.Create(newUser) as OkObjectResult;

			result.Should().NotBeNull();
			((User)result.Value).Id.Should().Be(10);

			_repoMock.Verify(r => r.AddAsync(newUser), Times.Once);
		}

		// ========== UPDATE ==========
		[Fact]
		public async Task Update_Should_Return_BadRequest_When_Id_Not_Match()
		{
			var user = new User { Id = 5, Username = "Mismatch" };

			var result = await _controller.Update(1, user);

			result.Should().BeOfType<BadRequestResult>();
		}

		[Fact]
		public async Task Update_Should_Update_User_When_Valid()
		{
			var user = new User { Id = 5, Username = "UpdateTest" };

			_repoMock.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);

			var result = await _controller.Update(5, user) as OkObjectResult;

			result.Should().NotBeNull();
			((User)result.Value).Id.Should().Be(5);

			_repoMock.Verify(r => r.UpdateAsync(user), Times.Once);
		}

		// ========== DELETE ==========
		[Fact]
		public async Task Delete_Should_Remove_User()
		{
			_repoMock.Setup(r => r.DeleteAsync(3)).Returns(Task.CompletedTask);

			var result = await _controller.Delete(3);

			result.Should().BeOfType<OkResult>();
			_repoMock.Verify(r => r.DeleteAsync(3), Times.Once);
		}
	}
}
