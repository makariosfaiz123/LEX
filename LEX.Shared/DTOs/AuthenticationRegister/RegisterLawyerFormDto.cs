using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LEX.Domain.EnumS;
using Microsoft.AspNetCore.Http;

namespace LEX.Shared.DTOs.AuthenticationRegister // يفضل تعدل النيم سبيس لـ Register بدل ForgotPassword
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

        // ==========================
        // بيانات الموقع (Location Data)
        // ==========================
        public int CountryId { get; set; }
        [Required]
        public int GovernorateId { get; set; }
        [Required]
        public int CityId { get; set; }
        public string? Street { get; set; }

        // ==========================
        // الملفات المطلوبة
        // ==========================
        [Required] public IFormFile NationalIdFront { get; set; } = null!;
        [Required] public IFormFile NationalIdBack { get; set; } = null!;
        [Required] public IFormFile SyndicateCard { get; set; } = null!;
    }
}