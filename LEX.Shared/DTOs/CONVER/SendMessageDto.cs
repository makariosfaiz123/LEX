using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Shared.DTOs.CONVER
{
    public class SendMessageDto
    {
        public int ConversationId { get; set; }
        public string Content { get; set; } // Text or Firebase URL
        public int MessageType { get; set; } // 1 for Text, 2 for File
        public string? FileName { get; set; }
    }
}
