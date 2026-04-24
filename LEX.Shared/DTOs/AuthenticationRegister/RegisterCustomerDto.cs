using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LEX.Domain.EnumS;

namespace LEX.Shared.DTOs.AuthenticationRegister
{
    public class RegisterCustomerDto
    {
        [Required] public string Fname { get; set; } = string.Empty;
        [Required] public string Lname { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required, MinLength(6)] public string Password { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public string? ProfileImage { get; set; }
    }
}
