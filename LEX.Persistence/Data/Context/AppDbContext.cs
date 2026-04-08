
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

        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Lawyer> Lawyers { get; set; }
        public DbSet<CaseRequests> CaseRequests { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Documents> Documents { get; set; }


    }
}
