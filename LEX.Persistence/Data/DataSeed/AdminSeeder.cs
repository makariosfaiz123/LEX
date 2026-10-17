using LEX.Domain.Entities;
using LEX.Domain.EnumS;
using LEX.Persistence.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LEX.Persistence.Data.DataSeed
{
    public static class AdminSeeder
    {
        /// <summary>
        /// Creates the first admin account from configuration (Admin:Email, Admin:Password, ...).
        /// Skipped when an admin already exists or when the credentials are not configured.
        /// </summary>
        public static async Task SeedAsync(AppDbContext context, IConfiguration configuration)
        {
            if (await context.Set<Users>().AnyAsync(u => u.RoleS == RoleS.Admin))
                return;

            var email = configuration["Admin:Email"];
            var password = configuration["Admin:Password"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("No admin account exists and Admin:Email / Admin:Password are not configured. Skipping admin seeding.");
                return;
            }

            var admin = new Users
            {
                Fname = configuration["Admin:FirstName"] ?? "Admin",
                Lname = configuration["Admin:LastName"] ?? "User",
                Email = email,
                Phone = configuration["Admin:Phone"] ?? "00000000000",
                RoleS = RoleS.Admin,
                Status = Status.Active,
                IsVerified = true,
                Date_Of_Birth = DateOnly.FromDateTime(DateTime.Now.AddYears(-25)),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };

            context.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}
