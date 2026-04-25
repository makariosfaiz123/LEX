using LEX.Domain.EnumS;
using LEX.Service.CaseRequests;
using LEX.Service_Abstraction.CaseRequests;
using LEX.Shared.DTOs.CaseRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LEX.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaseRequestsController : ControllerBase
    {
        private readonly ICaseRequestService _caseService;

        public CaseRequestsController(ICaseRequestService caseService)
        {
            _caseService = caseService;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("create-Customer")]
        public async Task<IActionResult> CreateCase(
            [FromForm] CreateCaseRequestDto dto,
            [FromForm] List<IFormFile> files
        )
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var mappedFiles = files?
                .Select(f => (f.OpenReadStream(), f.FileName))
                .ToList();

            var result = await _caseService.CreateCaseAsync(dto, userId, mappedFiles);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("case-types")]
        public IActionResult GetCaseTypes()
        {
            return Ok(Enum.GetNames(typeof(CaseType)));
        }

        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            return Ok(Enum.GetNames(typeof(Specializations)));
        }


        [Authorize(Roles = "Lawyer")]
        [HttpGet("my-requests-Lawyer")]
        public async Task<IActionResult> GetMyRequests([FromQuery] string? status)
        {
            var lawyerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var result = await _caseService.GetRequestsForLawyerAsync(lawyerId, status);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "Lawyer")]
        [HttpPost("HandleCase-Lawyer")]
        public async Task<IActionResult> HandleCase(HandleCaseRequestDto dto)
        {
            var lawyerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var result = await _caseService.HandleCaseRequestAsync(lawyerId, dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("my-cases-Customer")]
        public async Task<IActionResult> GetMyCases()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var result = await _caseService.GetCustomerCasesAsync(userId);

            return Ok(result);
        }

        [Authorize(Roles = "Lawyer")]
        [HttpGet("CaseDetailsFor-Lawyer/{id}")]
        public async Task<IActionResult> GetCaseDetailsForLawyer(int id)
        {
            var lawyerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var result = await _caseService.GetCaseDetailsForLawyerAsync(id, lawyerId);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("Case/{id}")]
        public async Task<IActionResult> GetCaseById(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var result = await _caseService.GetCaseDetailsAsync(id, userId);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }
}
