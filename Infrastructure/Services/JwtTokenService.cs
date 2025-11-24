using Application.Interfaces.External;
using Domain.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly byte[] _key;

        public JwtTokenService(IConfiguration configuration)
        {
            var jwtSecret = configuration["JwtSettings:Secret"] ?? "SuperSecretKeyForDevelopmentAndTestingOnly";
            _key = Encoding.ASCII.GetBytes(jwtSecret);
        }

        private string GenerateToken(User user, TimeSpan lifetime)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(lifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateUserToken(User user) => GenerateToken(user, TimeSpan.FromDays(7));
        public string GenerateVerificationToken(User user) => GenerateToken(user, TimeSpan.FromHours(24));

        public ClaimsPrincipal GetPrincipalFromToken(string token, bool isVerification)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(_key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = !isVerification
                };

                return tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            }
            catch { return null; }
        }
    }
}
