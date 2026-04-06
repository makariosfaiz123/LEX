using LEX.Domain.EnumS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Domain.Entities
{
    public class Lawyer : Users
    {
        #region (RS)
        public ICollection<CaseRequests> Case_Requests { get; set; } = [];
        public ICollection<Documents> Documents { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];

        #endregion


        public DateOnly Start_Practice { get; set; }

        public Specializations Specialization { get; set; }
        public Status Status { get; set; } = Status.Waiting;
    }
}
