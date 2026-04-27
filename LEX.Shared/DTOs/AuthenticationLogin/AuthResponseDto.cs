using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Shared.DTOs.AuthenticationLogin
{
    public class AuthResponseDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public int Id { get; set; }


    }
}
