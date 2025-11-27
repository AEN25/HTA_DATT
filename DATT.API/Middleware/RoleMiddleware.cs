using System.Security.Claims;

namespace DATT.API.Middleware
{
	public class RoleMiddleware
	{
		private readonly RequestDelegate _next;

		public RoleMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			// Nếu chưa đăng nhập → bỏ qua
			if (context.User.Identity?.IsAuthenticated != true)
			{
				await _next(context);
				return;
			}

			string? role = context.User.FindFirst(ClaimTypes.Role)?.Value;
			string? userId = context.User.FindFirst("id")?.Value;

			// Log thông tin user
			Console.WriteLine($"[Middleware] UserId={userId}, Role={role}, Path={context.Request.Path}");

			//❗ Chặn trường hợp USER cố truy cập API dành riêng cho admin
			if (context.Request.Path.StartsWithSegments("/api/task/admin") && role != "ADMIN")
			{
				context.Response.StatusCode = StatusCodes.Status403Forbidden;
				await context.Response.WriteAsync("Bạn không có quyền truy cập API này");
				return;
			}

			await _next(context);
		}
	}
}
