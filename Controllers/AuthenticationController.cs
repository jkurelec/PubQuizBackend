using Microsoft.AspNetCore.Mvc;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.UserDto;
using PubQuizBackend.Model.Other;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Util;
using PubQuizBackend.Util.Helpers;

namespace PubQuizBackend.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IJwtService _jwtService;
        private readonly ITeamService _teamService;

        public AuthenticationController(IUserService userService, IRefreshTokenService refreshTokenService, IJwtService jwtService, ITeamService teamService)
        {
            _userService = userService;
            _refreshTokenService = refreshTokenService;
            _jwtService = jwtService;
            _teamService = teamService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
        {
            if (!Request.Headers.TryGetValue("AppName", out var appName) && appName.FirstOrDefault() != "Attendee")
                return BadRequest("Invalid request");

            var intApp = CustomConverter.GetIntRole(appName.First()!);

            await _userService.ExistsByUsernameOrEmail(registerUserDto.Username, registerUserDto.Email);

            PasswordHelper.CreatePasswordHash(registerUserDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Username = registerUserDto.Username,
                Role = registerUserDto.Role,
                Rating = 1000,
                Firstname = registerUserDto.Firstname,
                Lastname = registerUserDto.Lastname,
                Email = registerUserDto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                ProfileImage = "default.jpg"
            };

            await _userService.Add(user);

            HttpContext.Response.Cookies.Append("refreshToken", await _refreshTokenService.Create(user.Id, user.Role, intApp), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTime.UtcNow.AddHours(_refreshTokenService.LongevityMultiplyer(user.Role))
            });

            return Ok(new
            {
                AccessToken = _jwtService.GenerateAccessToken(user.Id.ToString(), user.Username, user.Role, null, intApp)
            });
        }

        [HttpPost("changePassword")]
        public async Task<IActionResult> Register(ChangePassword changePassword)
        {
            await _userService.ChangePassword(changePassword.Id, changePassword.Password);
            return Ok("Password changed successfully.");
            //if (!Request.Headers.TryGetValue("AppName", out var appName) && appName.FirstOrDefault() != "Attendee")
            //    return BadRequest("Invalid request");

            //var intApp = CustomConverter.GetIntRole(appName.First()!);

            //await _userService.ExistsByUsernameOrEmail(registerUserDto.Username, registerUserDto.Email);

            //PasswordHelper.CreatePasswordHash(registerUserDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            //var user = new User
            //{
            //    Username = registerUserDto.Username,
            //    Role = registerUserDto.Role,
            //    Rating = 1000,
            //    Firstname = registerUserDto.Firstname,
            //    Lastname = registerUserDto.Lastname,
            //    Email = registerUserDto.Email,
            //    PasswordHash = passwordHash,
            //    PasswordSalt = passwordSalt
            //};

            //await _userService.Add(user);

            //HttpContext.Response.Cookies.Append("refreshToken", await _refreshTokenService.Create(user.Id, user.Role, intApp), new CookieOptions
            //{
            //    HttpOnly = true,
            //    Secure = true,
            //    SameSite = SameSiteMode.None,
            //    Path = "/",
            //    Expires = DateTime.UtcNow.AddHours(_refreshTokenService.LongevityMultiplyer(user.Role))
            //});

            //return Ok(new
            //{
            //    AccessToken = _jwtService.GenerateAccessToken(user.Id.ToString(), user.Username, user.Role, null, intApp)
            //});
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto loginUserDto)
        {
            var user = await _userService.GetByIdentifier(loginUserDto.Identifier);

            if (user == null)
                return BadRequest("Invalid username, email or password.");

            if (!PasswordHelper.VerifyPasswordHash(loginUserDto.Password, user.PasswordHash, user.PasswordSalt))
                return BadRequest("Invalid username, email or password.");

            if (!Request.Headers.TryGetValue("AppName", out var appName) && appName.FirstOrDefault() != "Attendee")
                return BadRequest("Invalid request");

            var intApp = CustomConverter.GetIntRole(appName.First()!);

            var teamId = await _teamService.GetIdByOwnerId(user.Id);

            HttpContext.Response.Cookies.Append("refreshToken", await _refreshTokenService.Create(user.Id, user.Role, intApp), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddHours(_refreshTokenService.LongevityMultiplyer(user.Role))
            });

            return Ok(new
            {
                AccessToken = _jwtService.GenerateAccessToken(user.Id.ToString(), user.Username, user.Role, teamId, intApp)
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            {
                HttpContext.Response.Cookies.Append("refreshToken", "", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    //Path = "/",
                    //Domain = "192.168.0.213",
                    Expires = DateTimeOffset.UtcNow.AddDays(-1)
                });

                return Forbid("Bearer");
            }
                

            var token = await _refreshTokenService.GetByToken(refreshToken);

            if (token == null)
            {
                HttpContext.Response.Cookies.Append("refreshToken", "", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    //Path = "/",
                    //Domain = "192.168.0.213",
                    Expires = DateTimeOffset.UtcNow.AddDays(-1)
                });

                return Forbid("Bearer");
            }

            if (token!.ExpiresAt < DateTime.UtcNow)
            {
                HttpContext.Response.Cookies.Append("refreshToken", "", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    //Path = "/",
                    //Domain = "192.168.0.213",
                    Expires = DateTimeOffset.UtcNow.AddDays(-1)
                });

                await _refreshTokenService.Delete(token);

                return Forbid("Bearer");
            }

            if (!Request.Headers.TryGetValue("AppName", out var appName) && appName.FirstOrDefault() != "")
                return BadRequest("Invalid request");

            var teamId = await _teamService.GetIdByOwnerId(token.UserId);

            return Ok(new
            {
                AccessToken = _jwtService.GenerateAccessToken(token.User.Id.ToString(), token.User.Username, token.User.Role, teamId, CustomConverter.GetIntRole(appName.First()!))
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
                return Unauthorized("Refresh token not found");

            var token = await _refreshTokenService.GetByToken(refreshToken);

            if (!Request.Headers.TryGetValue("AppName", out var appName) && appName.FirstOrDefault() != "")
                return BadRequest("Invalid request");

            if (token != null)
            {
                await _refreshTokenService.Delete(token);

                HttpContext.Response.Cookies.Append("refreshToken", "", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    //Path = "/",
                    //Domain = "192.168.0.213",
                    Expires = DateTimeOffset.UtcNow.AddDays(-1)
                });

                return Ok();
            }

            return Unauthorized("Nemože zločko");
        }
    }
}
