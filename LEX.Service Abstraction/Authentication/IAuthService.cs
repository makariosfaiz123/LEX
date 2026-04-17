using LEX.Shared.DTOs.AuthenticationRegister;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Service_Abstraction.Authentication
{
    public interface IAuthService
    {
        //Task<string> LoginAsync(string email, string password);
        Task<string> RegisterAsync(RegisterDto dto);
    }
}
