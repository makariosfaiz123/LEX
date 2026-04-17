
using LEX.Domain.Interfaces;
using LEX.Persistence.Data.Context;
using LEX.Persistence.Data.DataSeed;
using LEX.Persistence.Repositories;
using LEX.Service;
using LEX.Service_Abstraction.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LEX.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container.

            #region Add services to the container


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<DataSeeding, DataSeeding>();
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            // AutoMapper
            builder.Services.AddAutoMapper(typeof(AuthService).Assembly);


            //Unit of Work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Services
            builder.Services.AddScoped<IAuthService, AuthService>();




            #endregion

            var app = builder.Build();
            #region Dataseeding

            await using var scope = app.Services.CreateAsyncScope();
            var objectsToSeed = scope.ServiceProvider.GetRequiredService<DataSeeding>();
            await objectsToSeed.DataSeed();

            #endregion



            // Configure the HTTP request pipeline.
            #region Configure the HTTP request pipeline

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            //app.UseAuthorization();


            app.MapControllers();
            #endregion

            app.Run();
        }
    }
}
