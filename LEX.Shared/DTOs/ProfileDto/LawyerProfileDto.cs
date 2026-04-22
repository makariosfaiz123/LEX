using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Shared.DTOs.ProfileDto
{
    public class LawyerProfileDto : BaseProfileDto
    {
        [JsonPropertyOrder(7)]
        public string? About { get; set; }

        [JsonPropertyOrder(8)]
        public string? Education { get; set; }

        [JsonPropertyOrder(9)]
        public int YearsOfExperience { get; set; }

        [JsonPropertyOrder(10)]
        public List<DocumentDto> Documents { get; set; } = new();

        [JsonPropertyOrder(11)]
        public double Rating { get; set; }

        [JsonPropertyOrder(12)]
        public int ReviewsCount { get; set; }
    }
}
