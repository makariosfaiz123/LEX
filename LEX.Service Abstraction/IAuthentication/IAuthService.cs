using LEX.Shared.DTOs.AuthenticationLogin;
using LEX.Shared.DTOs.AuthenticationRegister;
using LEX.Shared.DTOs.EmailOTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Service_Abstraction.Authentication
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
        Task<string> RegisterAsync(RegisterDto dto);
        Task<string> VerifyOtpAsync(VerifyOtpDto dto);
    }
}
