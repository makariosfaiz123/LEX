using LEX.Domain.EnumS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Shared.DTOs.CaseRequest
{
    public class CreateCaseRequestDto
    {
        [Required]
        public string CaseType { get; set; }
        [Required]
        public int LawyerId { get; set; }

        [Required]
        [MinLength(10)]
        public string Description { get; set; }

        public List<string>? Documents { get; set; }
    }
}
