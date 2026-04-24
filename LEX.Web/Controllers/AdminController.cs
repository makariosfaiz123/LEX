using LEX.Domain.Entities;
using LEX.Domain.EnumS;
using LEX.Domain.Interfaces;
using LEX.Service_Abstraction.IAdminService;
using LEX.Shared.DTOs;
using LEX.Shared.DTOs.Admin;
using LEX.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LEX.Web.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        
        [HttpGet("pending-lawyers")]
        public async Task<IActionResult> GetPending()
        {
            var result = await _adminService.GetPendingLawyersAsync();
            return Ok(result);
        }

        
        [HttpPut("accept/{id}")]
        public async Task<IActionResult> Accept(int id)
        {
            var result = await _adminService.AcceptLawyerAsync(id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        
        [HttpPut("reject/{id}")]
        public async Task<IActionResult> Reject(int id, [FromBody] RejectLawyerDto dto)
        {
            var result = await _adminService.RejectLawyerAsync(id, dto.Reason);

            if (!result.Success)
                return BadRequest(result);  

            return Ok(result);
        }
    }
}
