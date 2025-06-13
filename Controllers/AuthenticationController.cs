using Microsoft.AspNetCore.Mvc;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto;
using PubQuizBackend.Model.Dto.UserDto;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Util;

namespace PubQuizBackend.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IJwtService _jwtService;

        public AuthenticationController(IUserService userService, IRefreshTokenService refreshTokenService, IJwtService jwtService)
        {
            _userService = userService;
            _refreshTokenService = refreshTokenService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
        {
            if (!Request.Headers.TryGetValue("AppName", out var appName) && appName.FirstOrDefault() != "Attendee")
                return BadRequest("Invalid request");

            var intApp = CustomConverter.GetIntRole(appName.First()!);

            var user = await _userService.Add(registerUserDto);

            if(user == null)
                return BadRequest("Username or Email already taken!");

            HttpContext.Response.Cookies.Append("refreshToken", await _refreshTokenService.Create(user.Id, user.Role, intApp), new CookieOptions
            {
                HttpOnly = true,
                //Secure = true,
                //SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(_refreshTokenService.LongevityMultiplyer(user.Role))
            });

            return Ok(new
            {
                AccessToken = _jwtService.GenerateAccessToken(user.Id.ToString(), user.Username, user.Role, intApp)
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto loginUserDto)
        {
            var user = await _userService.GetByIdentifier(loginUserDto.Identifier);

            if (user == null || user.Password != loginUserDto.Password)
                return BadRequest("Invalid username, email or password.");

            if (!Request.Headers.TryGetValue("AppName", out var appName) && appName.FirstOrDefault() != "")
                return BadRequest("Invalid request");

            var intApp = CustomConverter.GetIntRole(appName.First()!);

            HttpContext.Response.Cookies.Append("refreshToken", await _refreshTokenService.Create(user.Id, user.Role, intApp), new CookieOptions
            {
                HttpOnly = true,
                //Secure = true,
                //SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(_refreshTokenService.LongevityMultiplyer(user.Role))
            });

            return Ok(new
            {
                AccessToken = _jwtService.GenerateAccessToken(user.Id.ToString(), user.Username, user.Role, intApp)
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
                return Unauthorized("Refresh token not found");

            var token = await _refreshTokenService.GetByToken(refreshToken);

            if (!Request.Headers.TryGetValue("AppName", out var appName) && appName.FirstOrDefault() != "")
                return BadRequest("Invalid request");

            if (token != null)
                return Ok(new
                {
                    AccessToken = _jwtService.GenerateAccessToken(token.User.Id.ToString(), token.User.Username, token.User.Role, CustomConverter.GetIntRole(appName.First()!))
                });

            return Unauthorized("Nemože zločko");
        }
    }
}
