using LEX.Domain.EnumS;
using LEX.Shared.DTOs.AuthenticationForgotPassword;
using LEX.Shared.DTOs.AuthenticationLogin;
using LEX.Shared.DTOs.AuthenticationRegister;
using LEX.Shared.DTOs.EmailOTP;
using LEX.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Service_Abstraction.Authentication
{
    public interface IAuthService
    {
        Task<ServiceResponse<RegisterResponseDto>> RegisterCustomerAsync(RegisterCustomerDto dto);
        Task<ServiceResponse<RegisterResponseDto>> RegisterLawyerAsync(
            RegisterLawyerFormDto dto,
            List<(Stream Stream, string FileName, DocumentType Type)> documents);

        Task<ServiceResponse<string>> VerifyEmailLinkAsync(int userId, string token);
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
        //Task<string> RegisterAsync(RegisterDto dto);
        Task<ServiceResponse<RegisterResponseDto>> RegisterAsync(RegisterDto dto);
        Task<ServiceResponse<string>> RegisterDocumentsAsync(
                int lawyerId,
                List<(Stream Stream, string FileName, DocumentType Type)> documents
            );
        Task<string> VerifyOtpAsync(VerifyOtpDto dto);
        Task<string> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<string> ResetPasswordAsync(ResetPasswordDto dto);
        Task<string> UpdateFcmTokenAsync(int userId, UpdateFcmTokenDto dto);
    }
}
