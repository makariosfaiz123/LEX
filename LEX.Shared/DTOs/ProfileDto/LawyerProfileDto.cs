using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

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

        // ==========================
        // Location
        // ==========================
        [JsonPropertyOrder(13)]
        public string? Country { get; set; }

        [JsonPropertyOrder(14)]
        public string? Governorate { get; set; }

        [JsonPropertyOrder(15)]
        public string? City { get; set; }

        [JsonPropertyOrder(16)]
        public string? Street { get; set; }

        [JsonPropertyOrder(17)]
        public string FullLocation =>
            string.Join(", ",
                new[] { Street, City, Governorate, Country }
                .Where(x => !string.IsNullOrWhiteSpace(x)));
    }
}