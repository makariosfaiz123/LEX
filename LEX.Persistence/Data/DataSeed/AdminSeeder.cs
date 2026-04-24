using LEX.Domain.Entities;
using LEX.Domain.EnumS;
using LEX.Persistence.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Persistence.Data.DataSeed
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // ✔ لو فيه Admin موجود خلاص اخرج
            if (await context.Set<Users>().AnyAsync(u => u.RoleS == RoleS.Admin))
                return;

            var admin = new Users
            {
                Fname = "Admin",
                Lname = "Makarios",
                Email = "makariosfaiz0@gmail.com",
                Phone = "01208584569",
                RoleS = RoleS.Admin,
                Status = Status.Active,
                IsVerified = true,
                Date_Of_Birth = DateOnly.FromDateTime(DateTime.Now.AddYears(-25)),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456a")
            };

            context.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}
