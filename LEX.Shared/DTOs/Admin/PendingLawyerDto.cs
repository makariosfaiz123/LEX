using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Shared.DTOs.Admin
{
    public class PendingLawyerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string? ProfileImage { get; set; }

        public string? About { get; set; }
        public string? Education { get; set; }
        public int YearsOfExperience { get; set; }

        public List<DocumentDto> Documents { get; set; }
    }
}
