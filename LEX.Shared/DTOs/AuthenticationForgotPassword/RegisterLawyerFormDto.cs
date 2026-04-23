using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LEX.Domain.EnumS;
using Microsoft.AspNetCore.Http;

namespace LEX.Shared.DTOs.AuthenticationForgotPassword
{
    public class RegisterLawyerFormDto
    {
        [Required] public string Fname { get; set; } = string.Empty;
        [Required] public string Lname { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required, MinLength(6)] public string Password { get; set; } = string.Empty;
        [Required] public string Phone { get; set; } = string.Empty;

        public DateOnly? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public string? ProfileImage { get; set; }

        [Required] public DateOnly? StartPractice { get; set; }
        [Required] public List<string> Specializations { get; set; } = new();

        [Required] public IFormFile NationalIdFront { get; set; } = null!;
        [Required] public IFormFile NationalIdBack { get; set; } = null!;
        [Required] public IFormFile SyndicateCard { get; set; } = null!;
    }
}
