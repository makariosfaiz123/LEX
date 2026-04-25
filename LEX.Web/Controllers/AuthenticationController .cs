using LEX.Domain.EnumS;
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

        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register/customer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterCustomerDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _authService.RegisterCustomerAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("register/lawyer")]
        public async Task<IActionResult> RegisterLawyer([FromForm] RegisterLawyerFormDto form)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var documents = new List<(Stream Stream, string FileName, DocumentType Type)>
            {
                (form.NationalIdFront.OpenReadStream(), form.NationalIdFront.FileName, DocumentType.NationalIdFront),
                (form.NationalIdBack.OpenReadStream(), form.NationalIdBack.FileName, DocumentType.NationalIdBack),
                (form.SyndicateCard.OpenReadStream(), form.SyndicateCard.FileName, DocumentType.SyndicateCard)
            };

            var result = await _authService.RegisterLawyerAsync(form, documents);

            foreach (var doc in documents) doc.Stream.Dispose();

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] int userId, [FromQuery] string token)
        {
            var result = await _authService.VerifyEmailLinkAsync(userId, token);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // ==========================================
        // 3. تسجيل الدخول (نفس الكود بتاعك)
        // ==========================================
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

        // ==========================================
        // 4. تأكيد الـ OTP (نفس الكود بتاعك)
        // ==========================================
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpDto dto)
        {
            var result = await _authService.VerifyOtpAsync(dto);

            if (result != "Email verified successfully")
                return BadRequest(result);

            return Ok(result);
        }

        // ==========================================
        // 5. نسيان كلمة المرور (نفس الكود بتاعك)
        // ==========================================
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            var result = await _authService.ForgotPasswordAsync(dto);

            if (result != "OTP sent to your email")
                return BadRequest(result);

            return Ok(result);
        }
        [HttpGet("specializations")]
        public IActionResult GetSpecializations()
        {
            var specializations = Enum.GetValues(typeof(Specializations))
                .Cast<Specializations>()
                .Where(s => s != Specializations.None)
                .Select(s => new
                {
                    Id = (int)s,
                    Name = s.ToString()
                })
                .ToList();

            return Ok(specializations);
        }
        // ==========================================
        // 6. إعادة تعيين كلمة المرور (نفس الكود بتاعك)
        // ==========================================
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
