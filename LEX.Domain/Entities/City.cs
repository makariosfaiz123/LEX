using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LEX.Domain.Interfaces;

namespace LEX.Domain.Entities
{
    public class City : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int GovernorateId { get; set; }
        public Governorate Governorate { get; set; }
    }
}
