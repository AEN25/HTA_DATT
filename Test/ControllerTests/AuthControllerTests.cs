using DATT.API.Controllers;
using DATT.API.Data;
using DATT.API.DTOs.Auth;
using DATT.API.Helpers;
using DATT.API.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.ControllerTests
{
	public class AuthControllerTests
	{
		private AppDbContext GetDbContext()
		{
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;

			return new AppDbContext(options);
		}

		[Fact]
		public async Task Register_Should_Create_User()
		{
			var context = GetDbContext();

			var jwt = new Mock<JwtService>(MockBehavior.Loose, new ConfigurationBuilder().Build());

			var controller = new AuthController(context, jwt.Object);

			var req = new RegisterRequest
			{
				Name = "Test",
				Email = "t1@gmail.com",
				Password = "123"
			};

			var result = await controller.Register(req) as OkObjectResult;

			result.Should().NotBeNull();
			context.Users.Count().Should().Be(1);
		}

		[Fact]
		public async Task Login_Should_Return_Token_When_Valid()
		{
			var context = GetDbContext();

			// Seed user
			var user = new User
			{
				Username = "A",
				Email = "a@gmail.com",
				PasswordHash = BCrypt.Net.BCrypt.HashPassword("123"),
				Role = "USER"
			};
			context.Users.Add(user);
			await context.SaveChangesAsync();

			// fake jwt
			var jwtMock = new Mock<IJwtService>();
			jwtMock.Setup(x => x.GenerateToken(It.IsAny<User>()))
				   .Returns("fake-token-123");

			var controller = new AuthController(context, jwtMock.Object);


			var req = new LoginRequest
			{
				Email = "a@gmail.com",
				Password = "123"
			};

			var result = await controller.Login(req) as OkObjectResult;

			result.Should().NotBeNull();
			result!.Value!.ToString().Should().Contain("fake-token-123");
		}

		[Fact]
		public async Task Login_Should_Return_BadRequest_When_Invalid()
		{
			var context = GetDbContext();

			var jwt = new Mock<JwtService>(MockBehavior.Loose, new ConfigurationBuilder().Build());
			var controller = new AuthController(context, jwt.Object);

			var req = new LoginRequest
			{
				Email = "notfound@gmail.com",
				Password = "wrong"
			};

			var result = await controller.Login(req) as BadRequestObjectResult;

			result.Should().NotBeNull();
		}
	}
}
