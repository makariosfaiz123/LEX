using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LEX.Domain.Interfaces;
using LEX.Domain.EnumS;

namespace LEX.Domain.Entities
{
    public class Message : IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        public int ConversationId { get; set; }
        [ForeignKey(nameof(ConversationId))]
        public Conversation Conversation { get; set; }

        // The ID of the person who sent the message (Lawyer or Customer)
        public int SenderId { get; set; }

        // If Text: The message body. If File: The URL to the storage (Firebase/Azure)
        [Required]
        public string Content { get; set; }

        public MessageType Type { get; set; }

        // Optional: Store file details if Type is File
        public string? FileName { get; set; }
        public string? FileExtension { get; set; }
        public long? FileSizeInBytes { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
