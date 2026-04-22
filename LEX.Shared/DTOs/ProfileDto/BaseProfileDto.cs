using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Shared.DTOs.ProfileDto
{
    public class BaseProfileDto
    {
        [JsonPropertyOrder(1)]
        public int Id { get; set; }

        [JsonPropertyOrder(2)]
        public string Name { get; set; }

        [JsonPropertyOrder(3)]
        public string Email { get; set; }

        [JsonPropertyOrder(4)]
        public string? Phone { get; set; }

        [JsonPropertyOrder(5)]
        public string? ProfileImage { get; set; }

        [JsonPropertyOrder(6)]
        public string Role { get; set; }
    }
}
