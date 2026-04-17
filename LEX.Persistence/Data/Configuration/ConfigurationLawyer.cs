using LEX.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Domain.Configuration
{
    public class ConfigurationLawyer : IEntityTypeConfiguration<Lawyer>
    {
        public void Configure(EntityTypeBuilder<Lawyer> builder)
        {
            builder.HasMany(l => l.Case_Requests)
                 .WithOne(cr => cr.Lawyer)

                 .HasForeignKey(cr => cr.LawyerId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(l => l.Documents)
                .WithOne(d => d.Lawyer)
                .HasForeignKey(d => d.Lawyer_Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(l => l.Reviews)
                .WithOne(r => r.Lawyer)
                .HasForeignKey(r => r.LawyerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
