using LEX.Shared.DTOs.ReviewsDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Shared.DTOs.LawyerDtos
{
    public class LawyerDetailsDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<string> Specializations { get; set; } = new();

        public string? ProfileImage { get; set; }

        public int ReviewsCount { get; set; }

        public int YearsOfExperience { get; set; }

        public string About { get; set; } 
        public string Education { get; set; }
        public double Rating { get; set; }
        public List<ReviewDto> Reviews { get; set; } = new();
    }
}
