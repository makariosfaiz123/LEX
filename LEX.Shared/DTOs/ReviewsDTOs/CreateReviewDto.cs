using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Shared.DTOs.ReviewsDTOs
{
    public class CreateReviewDto
    {
        public int LawyerId { get; set; }

        public int Rating { get; set; }

        public string? Comment { get; set; }
    }
}
