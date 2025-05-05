using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Auth;
using PubQuizBackend.Model;
using PubQuizBackend.Models.Dto;
using PubQuizBackend.Utils;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PubQuizBackend.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly PubQuizContext _dbContext;
        private readonly JwtService _jwtService;

        public AuthenticationController(PubQuizContext dbContext, JwtService jwtService)
        {
            _dbContext = dbContext;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
        {
            if (!await _dbContext.Users.AnyAsync(x => x.Username == registerUserDto.Username || x.Email == registerUserDto.Email))
            {
                await _dbContext.Users.AddAsync(registerUserDto.ToUser());
                await _dbContext.SaveChangesAsync();
            }
            else
                return BadRequest("Username or Email already taken!");

            var user = await _dbContext.Users.Where(x => x.Username == registerUserDto.Username).FirstOrDefaultAsync();

            if (user != null)
            {
                var accessToken = _jwtService.GenerateAccessToken(user.Id.ToString(), user.Username, CustomConverter.GetStringRole(user.Role));
                var refreshToken = _jwtService.GenerateRefreshToken(user.Id, user.Role);

                return Ok(new
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            }

            return BadRequest("Something went wrong!");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto loginUserDto)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(
                x => x.Email == loginUserDto.Identifier || x.Username == loginUserDto.Identifier
            );

            if (user == null || user.Password != loginUserDto.Password)
                return BadRequest("Invalid username, email or password.");

            var accessToken = _jwtService.GenerateAccessToken(user.Id.ToString(), user.Username, CustomConverter.GetStringRole(user.Role));
            var refreshToken = await _jwtService.GenerateRefreshToken(user.Id, user.Role);

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenDto refreshToken)
        {
            var token = await _dbContext.RefreshTokens.Include(x => x.User).FirstOrDefaultAsync(x => x.Token == refreshToken.Value);

            if (token != null)
                return Ok(new
                {
                    AccessToken = _jwtService.GenerateAccessToken(token.User.Id.ToString(), token.User.Username, CustomConverter.GetStringRole(token.User.Role))
                });

            return Unauthorized("Nemože zločko");
        }
    }
}
