using LEX.Domain.Entities;
using LEX.Domain.Interfaces;
using LEX.Shared.DTOs.Location;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LEX.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public LocationsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("governorates")]
        public async Task<IActionResult> GetGovernorates()
        {
            var governorates = await _unitOfWork
             .GetRepository<Governorate, int>()
             .GetAsync(
                 includes: g => g.Cities
             );
            // بتجيب المحافظات ومعاها المدن بتاعتها

            // بنحولها لـ DTO
            var result = governorates.Select(g => new GovernorateDto
            {
                Id = g.Id,
                Name = g.Name,
                Cities = g.Cities.Select(c => new CityDto
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList()
            }).ToList();

            return Ok(new { success = true, data = result });
        }
    }
}
