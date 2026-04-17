using LEX.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Persistence.Data.Configuration
{
    public class ConfigurationUser : IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> builder)
        {
            // Primary Key
            builder.HasKey(u => u.User_Id);

            // Required fields
            builder.Property(u => u.Fname)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(u => u.Lname)
                   .HasMaxLength(50);

            builder.Property(u => u.Email)
                   .IsRequired();

            builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

            builder.Property(u => u.Phone)
            .HasMaxLength(15);

            // Password
            builder.Property(u => u.PasswordHash)
                   .IsRequired(false);

            // Enums
            builder.Property(u => u.RoleS)
                   .IsRequired();

            builder.Property(u => u.Status)
                   .IsRequired();

            builder.Property(u => u.gender)
                   .IsRequired();

            builder.HasDiscriminator<string>("UserType")
            .HasValue<Customer>("Customer")
            .HasValue<Lawyer>("Lawyer");
        }
    }
}
