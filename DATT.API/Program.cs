
using DATT.API.Data;
using DATT.API.Helpers;
using DATT.API.Middleware;
using DATT.API.Models;
using DATT.API.Models.Responses;
using DATT.API.Repositories;
using DATT.API.Repository;
using DATT.API.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;


namespace DATT.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllers()
			.AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.ReferenceHandler =
					System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
			})
			.ConfigureApiBehaviorOptions(options =>
			{
				options.InvalidModelStateResponseFactory = context =>
				{
					var errors = context.ModelState
						.Where(x => x.Value.Errors.Count > 0)
						.Select(x => new
						{
							Field = x.Key,
							Error = x.Value.Errors.First().ErrorMessage
						});

					return new BadRequestObjectResult(new ErrorResponse
					{
						StatusCode = 400,
						Message = "D? li?u không h?p l?",
						Errors = errors
					});
				};
			});



			//database
			builder.Services.AddDbContext<AppDbContext>(options =>
			{
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
			});

			//repository
			builder.Services.AddScoped<IGenericRepository<User>, UserRepository>();
			//builder.Services.AddScoped<IGenericRepository<TaskItem>, TaskRepository>();
			builder.Services.AddScoped<ITaskRepository, TaskRepository>();

			//jwt service
			builder.Services.AddSingleton<IJwtService, JwtService>();


			//authentication	
			builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options => {
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = false,
					ValidateIssuerSigningKey = true,
					ValidIssuer = builder.Configuration["Jwt:Issuer"],
					IssuerSigningKey = new SymmetricSecurityKey(
						Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
					)
				};
			});
			builder.Services.AddAuthorization(options =>
			{
				options.AddPolicy("AdminOnly", policy =>
					policy.RequireRole("ADMIN"));

				options.AddPolicy("UserOnly", policy =>
					policy.RequireRole("USER"));

				options.AddPolicy("AdminOrUser", policy =>
					policy.RequireRole("ADMIN", "USER"));
			});


			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(c =>
			{
				c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
				{
					Description = "Nh?p JWT token theo d?ng: {token}",
					Name = "Authorization",
					In = Microsoft.OpenApi.Models.ParameterLocation.Header,
					Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
					Scheme = "bearer"
				});

				c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
				{
					{
						new Microsoft.OpenApi.Models.OpenApiSecurityScheme
						{
							Reference = new Microsoft.OpenApi.Models.OpenApiReference
							{
								Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
								Id = "Bearer"
							}
						},
						Array.Empty<string>()
					}
				});
			});


			Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Debug()
			.WriteTo.Console() // log ra console
			.WriteTo.File("logs/log-.txt",
				rollingInterval: RollingInterval.Day,     // t?o file log theo ngày
				retainedFileCountLimit: 7,               // gi? log 7 ngày
				shared: true)
			.CreateLogger();

			builder.Host.UseSerilog();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseMiddleware<ExceptionMiddleware>();

			app.UseHttpsRedirection();

			app.UseAuthentication();

			app.UseMiddleware<RoleMiddleware>();

			app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}
	}
}
