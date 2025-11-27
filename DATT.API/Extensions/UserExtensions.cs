using System.Security.Claims;

namespace DATT.API.Extensions
{
	public static class UserExtensions
	{
		public static int GetUserId(this ClaimsPrincipal user)
		{
			return int.Parse(user.Claims.First(c => c.Type == "id").Value);
		}
	}
}
