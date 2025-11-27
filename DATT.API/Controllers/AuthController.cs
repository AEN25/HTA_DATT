using DATT.API.Data;
using DATT.API.DTOs.Auth;
using DATT.API.Helpers;
using DATT.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATT.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly IJwtService _jwt;

		public AuthController(AppDbContext context, IJwtService jwt)
		{
			_context = context;
			_jwt = jwt;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterRequest req)
		{
			if (await _context.Users.AnyAsync(x => x.Email == req.Email))
				return BadRequest("Email đã tồn tại");

			var user = new User
			{
				Username = req.Name,
				Email = req.Email,
				PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password)
			};

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			return Ok("Đăng ký thành công");
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginRequest req)
		{
			var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == req.Email);

			if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
				return BadRequest("Sai email hoặc mật khẩu");

			var token = _jwt.GenerateToken(user);

			return Ok(new { token });
		}
	}
}
