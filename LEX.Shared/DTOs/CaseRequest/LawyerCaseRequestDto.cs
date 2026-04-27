using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Shared.DTOs.CaseRequest
{
    public class LawyerCaseRequestDto
    {
        public int CaseId { get; set; }

        public string ClientName { get; set; }

        public string CaseType { get; set; }

        // Added the Status property
        public string Status { get; set; }
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<string> Documents { get; set; } = new();
    }
}
