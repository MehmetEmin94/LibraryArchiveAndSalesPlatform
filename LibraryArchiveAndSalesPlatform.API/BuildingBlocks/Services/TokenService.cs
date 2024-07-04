using LibraryArchiveAndSalesPlatform.API.BuildingBlocks.IServices;
using LibraryArchiveAndSalesPlatform.API.Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryArchiveAndSalesPlatform.API.BuildingBlocks.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _securityKey;

        public TokenService(IConfiguration config)
        {
            _config = config;
            _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));
        }

        public string CreateToken(User user, IList<string> userRoles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,user.Id)
            };

            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role,role)));

            var creds = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken
            (
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                expires : DateTime.Now.AddDays(7),
                claims: claims,
                signingCredentials: 
                new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]!)),SecurityAlgorithms.HmacSha256)

            );

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }
    }
}
