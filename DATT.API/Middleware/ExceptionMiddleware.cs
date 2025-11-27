using DATT.API.Extensions;
using DATT.API.Models.Responses;
using System.Text.Json;

namespace DATT.API.Middleware
{
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionMiddleware> _logger;

		public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (AppException ex)
			{
				_logger.LogWarning("App exception: {Message}", ex.Message);
				await HandleException(context, ex.StatusCode, ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Lỗi hệ thống");
				await HandleException(context, 500, "Lỗi hệ thống, vui lòng thử lại sau.");
			}

		}

		private async Task HandleException(HttpContext context, int statusCode, string message)
		{
			context.Response.StatusCode = statusCode;
			context.Response.ContentType = "application/json";

			var response = new ErrorResponse
			{
				StatusCode = statusCode,
				Message = message
			};

			await context.Response.WriteAsync(JsonSerializer.Serialize(response));
		}
	}
}
