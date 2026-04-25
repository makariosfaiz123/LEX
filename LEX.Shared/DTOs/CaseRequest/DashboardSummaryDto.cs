using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Shared.DTOs.CaseRequest
{
    public class DashboardSummaryDto
    {
        public int ClientsRequests { get; set; } // pending
        public int TotalCases { get; set; }
        public int AcceptedCases { get; set; }
        public int RejectedCases { get; set; }
        public int ClientsCount { get; set; }

        public object RecentCases { get; set; } = new();
    }
}
