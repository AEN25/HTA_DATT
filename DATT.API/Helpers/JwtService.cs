using DATT.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DATT.API.Helpers
{
	public class JwtService: IJwtService
	{
		private readonly string _secret;
		private readonly string _issuer;

		public JwtService(IConfiguration config)
		{
			_secret = config["Jwt:Key"];
			_issuer = config["Jwt:Issuer"];
		}

		public string GenerateToken(User user)
		{
			var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));

			var creds = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Email),
				new Claim("id", user.Id.ToString()),
				new Claim(ClaimTypes.Role,user.Role)
			};

			var token = new JwtSecurityToken(
				issuer: _issuer,
				audience: _issuer,
				claims: claims,
				expires: DateTime.UtcNow.AddHours(3),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
