using LEX.Domain.EnumS;
using LEX.Shared.DTOs.CaseRequest;
using LEX.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Service_Abstraction.CaseRequests
{
    public interface ICaseRequestService
    {
        Task<ServiceResponse<string>> CreateCaseAsync(CreateCaseRequestDto dto, int userId);

        Task<ServiceResponse<List<LawyerCaseRequestDto>>> GetRequestsForLawyerAsync(int lawyerId,string? status);

        Task<ServiceResponse<string>> HandleCaseRequestAsync(int lawyerId, HandleCaseRequestDto dto);

        Task<ServiceResponse<List<CustomerCaseDto>>> GetCustomerCasesAsync(int customerId);
    }
}
