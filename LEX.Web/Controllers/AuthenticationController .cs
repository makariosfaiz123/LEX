using LEX.Service_Abstraction.Authentication;
using LEX.Shared.DTOs.AuthenticationLogin;
using LEX.Shared.DTOs.AuthenticationRegister;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(dto);

            if ( result == "Lawyer data is required")
                return BadRequest(result);

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


    }
}
