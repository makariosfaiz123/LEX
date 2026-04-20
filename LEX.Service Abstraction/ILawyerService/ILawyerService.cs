using LEX.Shared.DTOs.LawyerDtos;
using LEX.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Service_Abstraction.ILawyerService
{
    public interface ILawyerService
    {

        Task<ServiceResponse<List<LawyerDto>>> GetLawyersAsync(LawyerFilterDto filter);

        Task<ServiceResponse<List<SpecializationDto>>> GetSpecializationsAsync();
    }
}
