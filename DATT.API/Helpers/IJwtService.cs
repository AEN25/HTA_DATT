using DATT.API.Models;

namespace DATT.API.Helpers
{
	public interface IJwtService
	{
		string GenerateToken(User user);
	}
}
