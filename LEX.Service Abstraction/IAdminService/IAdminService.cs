using LEX.Shared.DTOs.Admin;
using LEX.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Service_Abstraction.IAdminService
{
    public interface IAdminService
    {
        Task<ServiceResponse<List<PendingLawyerDto>>> GetPendingLawyersAsync();

        Task<ServiceResponse<string>> AcceptLawyerAsync(int lawyerId);

        Task<ServiceResponse<string>> RejectLawyerAsync(int lawyerId, string reason);

        Task<ServiceResponse<PendingLawyerDto>> GetLawyerDetailsAsync(int id);
        Task<ServiceResponse<AdminDashboardDto>> GetDashboardAsync();
    }
}
