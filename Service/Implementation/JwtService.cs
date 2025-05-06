using Microsoft.IdentityModel.Tokens;
using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PubQuizBackend.Service.Implementation
{
    public class JwtService
    {
        private readonly IConfiguration _config;
        private readonly PubQuizContext _dbContext;

        public JwtService(IConfiguration config, PubQuizContext dbContext)
        {
            _config = config;
            _dbContext = dbContext;
        }

        public string GenerateAccessToken(string userId, string username, string role/*, int port*/)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                //isti ako samo na ovaj api puca
                audience: GetAudienceString(role/*port*/),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(LongevityMultiplyer(CustomConverter.GetIntRole(role))),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GetAudiencePort(int port) =>
            port switch
            {
                1 => _config["Jwt:Audience:Attendee"]!,
                2 => _config["Jwt:Audience:Organizer"]!,
                3 => _config["Jwt:Audience:Admin"]!,
                _ => throw new ArgumentException("Unkown role")
            };

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
