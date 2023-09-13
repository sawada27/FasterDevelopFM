using Microsoft.Extensions.Configuration;

namespace FasterDevelopFM.Middleware.RateLimit
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //зЂВс
            var configuration = builder.Configuration;
            builder.Services.AddAIpRateLimiting(configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<RateLimitMiddleware>();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}