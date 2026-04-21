using LEX.Service_Abstraction.IReviewService;
using LEX.Shared.DTOs.ReviewsDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LEX.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("Add")]
        public async Task<IActionResult> AddReview([FromBody] CreateReviewDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var result = await _reviewService.AddReviewAsync(userId, dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "Customer")]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateReview([FromBody] UpdateReviewDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var result = await _reviewService.UpdateReviewAsync(userId, dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "Customer")]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var result = await _reviewService.DeleteReviewAsync(userId, id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
