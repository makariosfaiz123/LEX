
using LEX.Domain.Interfaces;
using LEX.Persistence.Data.Context;
using LEX.Persistence.Data.DataSeed;
using LEX.Persistence.Repositories;
using LEX.Service.Authentication;
using LEX.Service.CaseRequests;
using LEX.Service.LawyerService;
using LEX.Service.ReviewService;
using LEX.Service.Security;
using LEX.Service_Abstraction.Authentication;
using LEX.Service_Abstraction.CaseRequests;
using LEX.Service_Abstraction.IAuthentication;
using LEX.Service_Abstraction.ILawyerService;
using LEX.Service_Abstraction.IReviewService;
using LEX.Service_Abstraction.ISecurity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
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
            // Swagger configuration for JWT authentication
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter: Bearer YOUR_TOKEN"
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
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
                        new string[] {}
                    }
                });
            });

            builder.Services.AddScoped<DataSeeding, DataSeeding>();
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            // AutoMapper && Repos
            builder.Services.AddAutoMapper(typeof(AuthService).Assembly);
            builder.Services.AddScoped<ILawyerRepository, LawyerRepository>();


            //Unit of Work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<ICaseRequestService, CaseRequestService>();
            builder.Services.AddScoped<ILawyerService, LawyerService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();


            //Authorization
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes("THIS_IS_MY_SUPER_SECRET_KEY_1234567890"))
                };
            });

            // To serialize enums as strings in JSON responses
            builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters
                    .Add(new JsonStringEnumConverter());
            });

            builder.Services.AddAuthorization();



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

            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            #endregion

            app.Run();
        }
    }
}
