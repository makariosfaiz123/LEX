using LEX.Shared.DTOs.ProfileDto;
using LEX.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Service_Abstraction
{
    public interface IUserService
    {
        Task<ServiceResponse<string>> UpdateProfileAsync(int userId, UpdateUserProfileDto dto);

        Task<ServiceResponse<object>> GetMyProfileAsync(int userId);
    }
}
