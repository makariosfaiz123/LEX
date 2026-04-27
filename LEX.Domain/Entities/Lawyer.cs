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
        public ICollection<CaseRequest> Case_Requests { get; set; } = [];
        public ICollection<Documents> Documents { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];
        public ICollection<Conversation> Conversations { get; set; } = [];
        #endregion


        public DateOnly Start_Practice { get; set; }
        public Specializations Specialization { get; set; }
        public string About { get; set; }
        public string Education { get; set; }
        // ==========================
        // الربط مع جدول الـ Location
        // ==========================
        public int? LocationId { get; set; } // Foreign Key (Nullable عشان مش إجباري وقت التسجيل)
        public Location? Location { get; set; } // Navigation Property
        //public Status Status { get; set; } = Status.Waiting;
    }
}
