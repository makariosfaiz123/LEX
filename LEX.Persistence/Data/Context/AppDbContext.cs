
using LEX.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Persistence.Data.Context
{
    public class AppDbContext : DbContext

    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<CaseDocument>()
                .HasOne(d => d.CaseRequest)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.CaseRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<City>()
                .HasOne(c => c.Governorate)
                .WithMany(g => g.Cities)
                .HasForeignKey(c => c.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Governorate>()
                .HasOne(g => g.Country)
                .WithMany(c => c.Governorates)
                .HasForeignKey(g => g.CountryId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Lawyer> Lawyers { get; set; }
        public DbSet<CaseRequest> CaseRequests { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Documents> Documents { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        // ==========================
        // الجداول الجديدة (Locations)
        // ==========================
        public DbSet<Country> Countries { get; set; }
        public DbSet<Governorate> Governorates { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Location> Locations { get; set; } // لو قررت تعمل جدول منفصل للـ Location
    }

}
