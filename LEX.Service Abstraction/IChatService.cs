using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LEX.Application.DTOs.Chat;
using LEX.Shared.DTOs.CONVER;
using LEX.Shared.Responses;

namespace LEX.Service_Abstraction
{
    public interface IChatService
    {
        Task<ServiceResponse<IEnumerable<ConversationDto>>> GetUserConversationsAsync(int userId);

        // New Methods
        Task<ServiceResponse<IEnumerable<MessageDto>>> GetChatHistoryAsync(int conversationId);
        Task<ServiceResponse<MessageDto>> SendMessageAsync(int senderId, SendMessageDto dto);
    }
}
