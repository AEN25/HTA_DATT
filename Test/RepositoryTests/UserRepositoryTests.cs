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
	public class UserRepositoryTests
	{
		private AppDbContext GetDbContext()
		{
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;

			return new AppDbContext(options);
		}

		[Fact]
		public async Task AddAsync_Should_Add_User()
		{
			var context = GetDbContext();
			var repo = new UserRepository(context);

			var user = new User
			{
				Username = "TestUser",
				Email = "test@gmail.com",
				PasswordHash = "123",
				Role = "USER"
			};

			await repo.AddAsync(user);

			context.Users.Count().Should().Be(1);
		}

		[Fact]
		public async Task GetAllAsync_Should_Return_All_Users()
		{
			var context = GetDbContext();

			context.Users.AddRange(
				new User { Username = "A", Email = "a@gmail.com", PasswordHash = "x", Role = "USER" },
				new User { Username = "B", Email = "b@gmail.com", PasswordHash = "y", Role = "ADMIN" }
			);
			await context.SaveChangesAsync();

			var repo = new UserRepository(context);

			var result = await repo.GetAllAsync();

			result.Count().Should().Be(2);
		}

		[Fact]
		public async Task GetByIdAsync_Should_Return_Correct_User()
		{
			var context = GetDbContext();

			var user = new User
			{
				Id = 1,
				Username = "A",
				Email = "a@gmail.com",
				PasswordHash = "123",
				Role = "USER"
			};

			context.Users.Add(user);
			await context.SaveChangesAsync();

			var repo = new UserRepository(context);

			var result = await repo.GetByIdAsync(1);

			result.Should().NotBeNull();
			result!.Email.Should().Be("a@gmail.com");
		}

		[Fact]
		public async Task DeleteAsync_Should_Remove_User()
		{
			var context = GetDbContext();
			var repo = new UserRepository(context);

			var user = new User
			{
				Id = 5,
				Username = "A",
				Email = "a@gmail.com",
				PasswordHash = "123",
				Role = "USER"
			};

			context.Users.Add(user);
			await context.SaveChangesAsync();

			await repo.DeleteAsync(5);

			context.Users.Count().Should().Be(0);
		}
	}
}
