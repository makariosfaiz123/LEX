using LEX.Domain.Entities;
using LEX.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LEX.Domain.Entities
{
    public class Review : IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int LawyerId { get; set; }

       
        public  Customer Customer { get; set; } = null!;

        public  Lawyer Lawyer { get; set; } = null!;
    }
}