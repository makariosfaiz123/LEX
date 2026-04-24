using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LEX.Domain.Interfaces;

namespace LEX.Domain.Entities
{
    public class Country : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<Governorate> Governorates { get; set; } = new List<Governorate>();
    }
}
