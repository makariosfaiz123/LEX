using LEX.Domain.Entities;
using LEX.Domain.Interfaces;
using LEX.Persistence.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Persistence.Repositories
{
    public class LawyerRepository : ILawyerRepository
    {
        private readonly AppDbContext _context;

        public LawyerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Lawyer> GetLawyerWithDetailsAsync(int id)
        {
            return await _context.Lawyers
            .Include(l => l.Location).ThenInclude(loc => loc.Country)
            .Include(l => l.Location).ThenInclude(loc => loc.Governorate)
            .Include(l => l.Location).ThenInclude(loc => loc.City)
            .Include(l => l.Reviews).ThenInclude(r => r.Customer)
            .FirstOrDefaultAsync(l => l.Id == id);
        }

    }
}
