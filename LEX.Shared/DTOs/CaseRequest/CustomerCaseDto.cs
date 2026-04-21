using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Shared.DTOs.CaseRequest
{
    public class CustomerCaseDto
    {
        public int CaseId { get; set; }
        public string? LawyerName { get; set; }
        public string CaseType { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
