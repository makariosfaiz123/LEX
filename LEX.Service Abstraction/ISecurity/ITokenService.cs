using LEX.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Service_Abstraction.ISecurity
{
    public interface ITokenService
    {
        string CreateToken(Users user);
    }
}
