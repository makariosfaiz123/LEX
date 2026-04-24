using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LEX.Domain.EnumS;

namespace LEX.Shared.DTOs.LawyerDtos
{
    public class LawyerSeedDto
    {
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateOnly Date_Of_Birth { get; set; }
        public Gender Gender { get; set; }
        public string Porfile_Image { get; set; }
        public Status Status { get; set; }
        public RoleS RoleS { get; set; }
        public DateOnly Start_Practice { get; set; }
        public int Specialization { get; set; }
        public string Country { get; set; }
        public string Governorate { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
    }
}
