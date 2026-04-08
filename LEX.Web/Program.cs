
using LEX.Persistence.Data.Context;
using LEX.Persistence.Data.DataSeed;
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

            #endregion

            var app = builder.Build();
            #region Dataseeding

            using var scope = app.Services.CreateScope();
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
