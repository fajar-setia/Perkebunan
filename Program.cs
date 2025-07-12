using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Perkebunan.Data;

namespace Perkebunan
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ✅ CORS config: nama harus sama dengan yang dipakai di UseCors
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", policy =>
                {
                    policy.WithOrigins("http://localhost:5174") // Ganti sesuai port React kamu
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
            var portEnv = Environment.GetEnvironmentVariable("PORT");
            if (int.TryParse(portEnv, out var port))
            {
                builder.WebHost.ConfigureKestrel(opts => {
                    opts.ListenAnyIP(port);
                });
            }

            var app = builder.Build();
            app.UseStaticFiles();

            // ✅ Aktifkan CORS
            app.UseCors("AllowReactApp");

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            
            app.MapControllers();

            app.Run();
        }
    }
}
