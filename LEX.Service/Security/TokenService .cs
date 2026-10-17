using LEX.Domain.Entities;
using LEX.Service_Abstraction.ISecurity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LEX.Service.Security
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration configuration)
        {
            var secret = configuration["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(secret) || secret.Length < 32)
                throw new InvalidOperationException(
                    "Missing or too short configuration value 'Jwt:Key' (min 32 characters). " +
                    "Set it with user-secrets or an environment variable.");

            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        }

        public string CreateToken(Users user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.RoleS.ToString())
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
