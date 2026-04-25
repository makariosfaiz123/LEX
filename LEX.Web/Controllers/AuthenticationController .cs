using LEX.Domain.Entities;
using LEX.Domain.EnumS;
using LEX.Domain.Interfaces;
using LEX.Service_Abstraction.Authentication;
using LEX.Shared.DTOs.AuthenticationForgotPassword;
using LEX.Shared.DTOs.AuthenticationLogin;
using LEX.Shared.DTOs.AuthenticationRegister;
using LEX.Shared.DTOs.EmailOTP;
using LEX.Web.UploadProfailDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LEX.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthenticationController(IAuthService authService , IUnitOfWork unitOfWork)
        {
            _authService = authService;
            _unitOfWork = unitOfWork;
        }


        [HttpGet("check-status")]
        public async Task<IActionResult> CheckStatus(int userId)
        {
            var userRepo = _unitOfWork.GetRepository<Users, int>();

            var user = await userRepo.GetByIdAsync(userId);

            if (user == null)
                return NotFound("User not found");

            if (user.RoleS != RoleS.Lawyer)
                return BadRequest("Not a lawyer");

            var lawyer = user as Lawyer;

            return Ok(new
            {
                status = lawyer.Status.ToString(),
                reason = lawyer.RejectionReason ?? "",
                canResubmit = lawyer.Status == Status.Blocked
            });
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }



        [HttpPost("register-documents")]
        public async Task<IActionResult> RegisterDocuments(
                [FromForm] int lawyerId,
                [FromForm] List<IFormFile> files,
                [FromForm] List<string> types
            )
        {
            if (files == null || types == null || files.Count != types.Count)
                return BadRequest("Each file must have a corresponding type");

            var mappedDocs = new List<(Stream, string, DocumentType)>();

            for (int i = 0; i < files.Count; i++)
            {
                if (!Enum.TryParse<DocumentType>(types[i], out var parsedType))
                    return BadRequest($"Invalid type: {types[i]}");

                mappedDocs.Add((
                    files[i].OpenReadStream(),
                    files[i].FileName,
                    parsedType
                ));
            }

            var result = await _authService.RegisterDocumentsAsync(
                lawyerId,
                mappedDocs
            );

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(dto);

            if (result == null)
                return Unauthorized("Invalid Email or Password");

            return Ok(result);
        }


        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpDto dto)
        {
            var result = await _authService.VerifyOtpAsync(dto);

            if (result != "Email verified successfully")
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            var result = await _authService.ForgotPasswordAsync(dto);

            if (result != "OTP sent to your email")
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var result = await _authService.ResetPasswordAsync(dto);

            if (result != "Password reset successfully")
                return BadRequest(result);

            return Ok(result);
        }


    }
}
