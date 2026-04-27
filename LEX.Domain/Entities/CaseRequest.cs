using LEX.Domain.EnumS;
using LEX.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Domain.Entities
{
    public class CaseRequest : IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int LawyerId { get; set; }
        public Lawyer Lawyer { get; set; }

        public CaseType Case_Type { get; set; }

        [Required]
        public string Description { get; set; }

        public CaseStatus Status { get; set; } = CaseStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<CaseDocument> Documents { get; set; } = new List<CaseDocument>();
        // Navigation property linking the case to its Firebase conversation metadata
        public Conversation? Conversation { get; set; }
    }
}
