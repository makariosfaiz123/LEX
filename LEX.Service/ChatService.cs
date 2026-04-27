using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LEX.Application.DTOs.Chat;
using LEX.Domain.Entities;
using LEX.Domain.EnumS;
using LEX.Domain.Interfaces;
using LEX.Service_Abstraction;
using LEX.Shared.DTOs.CONVER;
using LEX.Shared.Responses;
using Microsoft.AspNetCore.SignalR;

namespace LEX.Service
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<ChatHub> _hubContext; // Add this
        public ChatService(IUnitOfWork unitOfWork, IHubContext<ChatHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }



        public async Task<ServiceResponse<IEnumerable<ConversationDto>>> GetUserConversationsAsync(int userId)
        {
            var conversationRepo = _unitOfWork.GetRepository<Conversation, int>();

            // ==========================================
            // FIX: Using your GetAsync signature properly
            // ==========================================
            var conversations = await conversationRepo.GetAsync(
                filter: c => (c.LawyerId == userId || c.CustomerId == userId) && c.IsActive, // 1. Filter
                orderBy: q => q.OrderByDescending(c => c.CreatedAt),                         // 2. OrderBy
                page: null,                                                                  // 3. Page
                pageSize: null,                                                              // 4. PageSize
                c => c.Lawyer,                                                               // 5. Includes (params array)
                c => c.Customer,
                c => c.CaseRequest
            );

            // Map the entities to DTOs
            var conversationDtos = conversations.Select(c =>
            {
                // Determine who the "other" user is to display their name/picture
                bool isUserLawyer = c.LawyerId == userId;

                var otherUser = isUserLawyer ? (Users)c.Customer : (Users)c.Lawyer;
                string role = isUserLawyer ? "Customer" : "Lawyer";

                return new ConversationDto
                {
                    Id = c.Id,
                    FirebaseChatId = c.FirebaseChatId ?? "",
                    CaseRequestId = c.CaseRequestId,
                    CaseDescription = c.CaseRequest?.Description ?? "No Description",
                    OtherUserId = otherUser.Id,
                    OtherUserName = $"{otherUser.Fname} {otherUser.Lname}".Trim(),
                    OtherUserImage = otherUser.Porfile_Image ?? "",
                    OtherUserRole = role,
                    CreatedAt = c.CreatedAt
                };
            }).ToList();

            return ServiceResponse<IEnumerable<ConversationDto>>.SuccessResult(conversationDtos);
        }
        public async Task<ServiceResponse<IEnumerable<MessageDto>>> GetChatHistoryAsync(int conversationId)
        {
            var messageRepo = _unitOfWork.GetRepository<Message, int>();

            var messages = await messageRepo.GetAsync(
                filter: m => m.ConversationId == conversationId,
                orderBy: q => q.OrderBy(m => m.SentAt)
            );

            var messageDtos = messages.Select(m => new MessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                Content = m.Content,
                Type = m.Type.ToString(),
                FileName = m.FileName,
                SentAt = m.SentAt
            });

            return ServiceResponse<IEnumerable<MessageDto>>.SuccessResult(messageDtos);
        }

        public async Task<ServiceResponse<MessageDto>> SendMessageAsync(int senderId, SendMessageDto dto)
        {
            var conversationRepo = _unitOfWork.GetRepository<Conversation, int>();
            var messageRepo = _unitOfWork.GetRepository<Message, int>();

            var conversation = await conversationRepo.GetByIdAsync(dto.ConversationId);

            // FIX: Changed .FailureResult to .Fail to match your ServiceResponse class
            if (conversation == null || (conversation.LawyerId != senderId && conversation.CustomerId != senderId))
            {
                return ServiceResponse<MessageDto>.Fail("Conversation not found or access denied.");
            }

            var newMessage = new Message
            {
                ConversationId = dto.ConversationId,
                SenderId = senderId,
                Content = dto.Content,
                Type = (MessageType)dto.MessageType, // Cast int to Enum
                FileName = dto.FileName,
                SentAt = DateTime.UtcNow
            };

            await messageRepo.AddAsync(newMessage);
            await _unitOfWork.SaveChangesAsync();

            var resultDto = new MessageDto
            {
                Id = newMessage.Id,
                SenderId = senderId,
                Content = newMessage.Content,
                Type = newMessage.Type.ToString(),
                FileName = newMessage.FileName,
                SentAt = newMessage.SentAt
            };
            await _hubContext.Clients.Group(dto.ConversationId.ToString())
                .SendAsync("ReceiveMessage", resultDto);
            return ServiceResponse<MessageDto>.SuccessResult(resultDto);
        }
    }
}

