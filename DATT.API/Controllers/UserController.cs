using DATT.API.Data;
using DATT.API.Models;
using DATT.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATT.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IGenericRepository<User> _repo;

		public UserController(IGenericRepository<User> repo)
		{
			_repo = repo;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
			=> Ok(await _repo.GetAllAsync());

		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			var user = await _repo.GetByIdAsync(id);
			return user == null ? NotFound() : Ok(user);
		}

		[HttpPost]
		public async Task<IActionResult> Create(User user)
		{
			await _repo.AddAsync(user);
			return Ok(user);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, User user)
		{
			if (id != user.Id) return BadRequest();
			await _repo.UpdateAsync(user);
			return Ok(user);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			await _repo.DeleteAsync(id);
			return Ok();
		}
	}
}
