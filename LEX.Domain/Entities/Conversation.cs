using LEX.Domain.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LEX.Domain.Entities
{
    public class Conversation : IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        // The reference key to the Firebase collection/document where messages are actually stored
        public string? FirebaseChatId { get; set; }

        // Linked to the specific case/request that created this conversation
        public int CaseRequestId { get; set; }
        [ForeignKey(nameof(CaseRequestId))]
        public CaseRequest CaseRequest { get; set; }

        // The Lawyer participating in the conversation
        public int LawyerId { get; set; }
        [ForeignKey(nameof(LawyerId))]
        public Lawyer Lawyer { get; set; }

        // The Customer participating in the conversation
        public int CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}