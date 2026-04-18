using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Domain.Entities
{
    public class CaseDocument
    {
        public int Id { get; set; }
        public string FileUrl { get; set; }

        public int CaseId { get; set; }
        public CaseRequest CaseRequest { get; set; }
    }
}
