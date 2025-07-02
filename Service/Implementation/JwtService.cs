using Microsoft.IdentityModel.Tokens;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Util;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PubQuizBackend.Service.Implementation
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateAccessToken(string userId, string username, int role, int? teamId, int app)
        {
            var stringRole = CustomConverter.GetStringRole(role);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, stringRole),
                new Claim("teamId", teamId?.ToString() ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: GetAudienceString(CustomConverter.GetStringRole(app)),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(LongevityMultiplyer(app)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GetAudienceString(string role) =>
            role switch
            {
                "Attendee" => _config["Jwt:Audience:Attendee"]!,
                "Organizer" => _config["Jwt:Audience:Organizer"]!,
                "Admin" => _config["Jwt:Audience:Admin"]!,
                _ => throw new ArgumentException("Unkown role")
            };

        private static int LongevityMultiplyer(int role) =>
            role switch
            {
                1 => 15,
                2 => 5,
                3 => 1,
                _ => 0
            };
    }
}
