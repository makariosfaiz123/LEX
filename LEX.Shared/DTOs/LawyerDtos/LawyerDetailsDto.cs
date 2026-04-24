using LEX.Shared.DTOs.ReviewsDTOs;
using System;
using System.Collections.Generic;

namespace LEX.Shared.DTOs.LawyerDtos
{
    public class LawyerDetailsDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<string> Specializations { get; set; } = new();

        public string? ProfileImage { get; set; }

        public int ReviewsCount { get; set; }

        public int YearsOfExperience { get; set; }

        public string About { get; set; } = string.Empty;

        public string Education { get; set; } = string.Empty;

        public double Rating { get; set; }

        // ==========================
        // Location
        // ==========================
        public string? Country { get; set; }

        public string? Governorate { get; set; }

        public string? City { get; set; }

        public string? Street { get; set; }

        // جاهز للعرض مباشرة في Flutter
        public string FullLocation =>
            string.Join(", ",
                new[] { Street, City, Governorate, Country }
                .Where(x => !string.IsNullOrWhiteSpace(x)));

        public List<ReviewDto> Reviews { get; set; } = new();
    }
}