using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LEX.Domain.Interfaces;

namespace LEX.Domain.Entities
{
    public class Governorate : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int CountryId { get; set; }
        public Country Country { get; set; }

        public ICollection<City> Cities { get; set; } = new List<City>();
    }
}
