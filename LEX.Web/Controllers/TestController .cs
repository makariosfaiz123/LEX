using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LEX.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [Authorize]
        [HttpGet("auth")]
        public IActionResult TestAuth()
        {
            return Ok("You are authorized ✅");
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("customer")]
        public IActionResult CustomerOnly()
        {
            return Ok("Customer access ✅");
        }

        [Authorize(Roles = "Lawyer")]
        [HttpGet("lawyer")]
        public IActionResult LawyerOnly()
        {
            return Ok("Lawyer access ✅");
        }

        [Authorize]
        [HttpGet("current-user")]
        public IActionResult GetCurrentUser()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
            {
                userId,
                email,
                role
            });
        }
    }
}
