using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Model;
using PubQuizBackend.Model.Dto.UserDto;
using PubQuizBackend.Models.Dto;
using PubQuizBackend.Service.Implementation;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Utils;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PubQuizBackend.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly JwtService _jwtService;

        public AuthenticationController(IUserService userService, IRefreshTokenService refreshTokenService, JwtService jwtService)
        {
            _userService = userService;
            _refreshTokenService = refreshTokenService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
        {
            var user = await _userService.Add(registerUserDto);

            if(user == null)
                return BadRequest("Username or Email already taken!");

            //var port = HttpContext.Connection.RemotePort;
            return Ok(new
            {
                AccessToken = _jwtService.GenerateAccessToken(user.Id.ToString(), user.Username, CustomConverter.GetStringRole(user.Role)/*, port*/),
                RefreshToken = await _refreshTokenService.Create(user.Id, user.Role)
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto loginUserDto)
        {
            var user = await _userService.GetByIdentifier(loginUserDto.Identifier);

            if (user == null || user.Password != loginUserDto.Password)
                return BadRequest("Invalid username, email or password.");

            //var port = HttpContext.Connection.RemotePort;

            return Ok(new
            {
                AccessToken = _jwtService.GenerateAccessToken(user.Id.ToString(), user.Username, CustomConverter.GetStringRole(user.Role)/*, port*/),
                RefreshToken = await _refreshTokenService.Create(user.Id, user.Role)
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenDto refreshToken)
        {
            var token = await _refreshTokenService.GetByToken(refreshToken.Value);

            //var port = HttpContext.Connection.RemotePort;

            if (token != null)
                return Ok(new
                {
                    AccessToken = _jwtService.GenerateAccessToken(token.User.Id.ToString(), token.User.Username, CustomConverter.GetStringRole(token.User.Role)/*, port*/)
                });

            return Unauthorized("Nemože zločko");
        }
    }
}
