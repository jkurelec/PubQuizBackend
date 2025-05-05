using Microsoft.IdentityModel.Tokens;
using PubQuizBackend.Model;
using PubQuizBackend.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PubQuizBackend.Auth
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

        public string GenerateAccessToken(string userId, string username, string role)
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
                audience: _config["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(AccessTokenLongevityMultiplyer(CustomConverter.GetIntRole(role))),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GenerateRefreshToken(int userId, int role)
        {
            var tokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var expiresAt = DateTime.Now.AddHours(RefreshTokenLongevityMultiplyer(role));
            var token = await _dbContext.RefreshTokens.FindAsync(userId);

            if (token == null)
                await _dbContext.RefreshTokens.AddAsync(
                    new()
                    {
                        UserId = userId,
                        Token = tokenValue,
                        ExpiresAt = DateTime.Now.AddHours(RefreshTokenLongevityMultiplyer(role))
                    }
                );
            else
            {
                token.Token = tokenValue;
                token.ExpiresAt = expiresAt;
            }

            await _dbContext.SaveChangesAsync();

            return tokenValue;
        }

        private static int AccessTokenLongevityMultiplyer(int role) =>
            role switch
            {
                1 => 15,
                2 => 5,
                3 => 1,
                _ => 0
            };

        private static int RefreshTokenLongevityMultiplyer(int role) =>
            role switch
            {
                1 => 168,
                2 => 24,
                3 => 1,
                _ => 0
            };
    }
}
