using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Domain.Entities
{
    public class Customer : Users
    {
        public ICollection<CaseRequest> Case_Requests { get; set; } = [];
            public ICollection<Review> Reviews { get; set; } = [];

    }
}
    