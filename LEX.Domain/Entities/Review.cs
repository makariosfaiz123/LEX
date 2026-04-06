using LEX.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LEX.Domain.Entities
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "التقييم يجب أن يكون بين 1 و 5 نجوم")]
        public int Rating { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; }

        // Foreign Keys
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int LawyerId { get; set; }

       
        public  Customer Customer { get; set; } = null!;

        public  Lawyer Lawyer { get; set; } = null!;
    }
}