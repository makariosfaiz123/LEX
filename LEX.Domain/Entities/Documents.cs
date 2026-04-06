using LEX.Domain.EnumS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Domain.Entities
{
    public class Documents
    {


        #region (RS)
        public int Lawyer_Id { get; set; }
        public Lawyer Lawyer { get; set; }
        #endregion
        public int Id { get; set; }
        public DocumentType Document_Type { get; set; }
        public string Document_URL { get; set; }
        
        
    }
}
