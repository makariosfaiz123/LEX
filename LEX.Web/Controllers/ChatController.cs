using System.Security.Claims;
using LEX.Domain.Entities;
using LEX.Domain.Interfaces;
using LEX.Persistence.Repositories;
using LEX.Service;
using LEX.Service_Abstraction;
using LEX.Shared.DTOs.CONVER;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LEX.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requires a valid JWT Token
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IUnitOfWork _unitOfWork;

        public ChatController(IChatService chatService, IUnitOfWork unitOfWork)
        {
            _chatService = chatService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("my-conversations")]
        public async Task<IActionResult> GetMyConversations()
        {
            // 1. Extract the User ID from the Token
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { Message = "Invalid token or user ID not found." });
            }

            // 2. Call the service
            var response = await _chatService.GetUserConversationsAsync(userId);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        // 1. Get Chat History from SQL
        [HttpGet("history/{conversationId}")]
        public async Task<IActionResult> GetChatHistory(int conversationId)
        {
            var response = await _chatService.GetChatHistoryAsync(conversationId);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        // 2. Send Message (Saves to SQL + Triggers SignalR)
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            // We pass the userId from the token as the SenderId for security
            var response = await _chatService.SendMessageAsync(userId.Value, dto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        // Helper to get UserId from JWT
        private int? GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return null;
        }

    }


}
