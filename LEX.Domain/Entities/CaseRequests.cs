using LEX.Domain.EnumS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Domain.Entities
{
    public class CaseRequests
    {


        #region (RS)

        public int CustomerId { get; set; }

        public int LawyerId { get; set; }
        
        public Customer Customer { get; set; }
        public Lawyer Lawyer { get; set; }
        #endregion
        [Key]
        public int Case_Id { get; set; }

        public CaseType Case_Type { get; set; }

        public string Description { get; set; }
        // public string Case_Status { get; set; }
        public Collection<string> Attached_Files { get; set; } = [];



    }
}
