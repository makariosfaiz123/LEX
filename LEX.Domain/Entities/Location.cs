using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LEX.Domain.Interfaces;

namespace LEX.Domain.Entities
{
    public class Location : IEntity<int>
    {
        public int Id { get; set; }

        public int CountryId { get; set; }
        public Country Country { get; set; }

        public int GovernorateId { get; set; }
        public Governorate Governorate { get; set; }

        public int CityId { get; set; }
        public City City { get; set; }

        public string? Street { get; set; }

        public ICollection<Lawyer> Lawyers { get; set; } = new List<Lawyer>();
    }
}
