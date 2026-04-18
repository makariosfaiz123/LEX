using LEX.Domain.EnumS;
using LEX.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LEX.Domain.Entities
{
    public class Users : IEntity<int>
    {
        [Key]
        public int User_Id { get; set; }

        public Gender gender { get; set; }
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Phone]
        public string? Phone { get; set; }
        public DateOnly Date_Of_Birth { get; set; }

        public string? Porfile_Image { get; set; }
        public Status Status { get; set; }
        public RoleS RoleS { get; set; }
        public string PasswordHash { get; set; }
        public string? OtpCode { get; set; }
        public DateTime? OtpExpiry { get; set; }
        public bool IsVerified { get; set; } = false;

    }


}
