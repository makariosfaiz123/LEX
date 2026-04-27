using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Shared.DTOs.CONVER
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; } // Text content or File URL
        public string Type { get; set; }    // "Text" or "File"
        public string? FileName { get; set; }
        public DateTime SentAt { get; set; }
    }
}
