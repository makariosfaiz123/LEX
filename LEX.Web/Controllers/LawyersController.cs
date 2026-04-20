using LEX.Service_Abstraction.ILawyerService;
using LEX.Shared.DTOs.LawyerDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LEX.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LawyersController : ControllerBase
    {
        private readonly ILawyerService _lawyerService;

        public LawyersController(ILawyerService lawyerService )
        {
            _lawyerService = lawyerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLawyers([FromQuery] LawyerFilterDto filter)
        {
            var result = await _lawyerService.GetLawyersAsync(filter);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _lawyerService.GetSpecializationsAsync();

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }


        [HttpGet("Details/{id}")]
        public async Task<IActionResult> GetLawyerDetails(int id)
        {
            var result = await _lawyerService.GetLawyerDetailsAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }
}
