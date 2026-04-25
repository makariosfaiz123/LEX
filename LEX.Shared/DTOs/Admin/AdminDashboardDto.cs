using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Shared.DTOs.Admin
{
    public class AdminDashboardDto
    {
        public int TotalLawyers { get; set; }
        public int TotalCustomers { get; set; }
        public int NewRequests { get; set; }
    }
}
