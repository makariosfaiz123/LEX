using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Shared.DTOs.LawyerDtos
{
    public class LawyerDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<string> Specializations { get; set; } = new();

        public string? ProfileImage { get; set; }

        public double Rating { get; set; }   // ⭐

        public int YearsOfExperience { get; set; } // 🕐

    }
}
