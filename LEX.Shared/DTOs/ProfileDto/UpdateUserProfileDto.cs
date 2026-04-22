using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Shared.DTOs.ProfileDto
{
    public class UpdateUserProfileDto
    {
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public string? Phone { get; set; }

        public string? About { get; set; }
        public string? Education { get; set; }

        public string? ProfileImageUrl { get; set; } 
    }
}
