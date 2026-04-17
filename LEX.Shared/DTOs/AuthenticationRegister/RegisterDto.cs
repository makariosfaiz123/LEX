using LEX.Domain.EnumS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Shared.DTOs.AuthenticationRegister
{
    public class RegisterDto
    {
        [Required]
        public required string Fname { get; set; }

        public string? Lname { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.(com|net|org|eg)$",
        ErrorMessage = "Email must be valid domain (.com, .net, .org, .eg) Or (@)")]
        public required string Email { get; set; }

        [Required]
        [MinLength(6)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,}$",
        ErrorMessage = "Password must contain letters and numbers")]
        public required string Password { get; set; }

        [Required]
        [Phone]
        [RegularExpression(@"^01[0-2,5]{1}[0-9]{8}$",
        ErrorMessage = "Invalid Egyptian phone number")]
        public required string Phone { get; set; }

        [Required]
        public RoleS Role { get; set; }

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        public Gender Gender { get; set; }

        public string? ProfileImage { get; set; }

        // Lawyer only
        public DateOnly? StartPractice { get; set; }
        public Specializations? Specialization { get; set; }
    }
}
