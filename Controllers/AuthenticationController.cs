using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Auth;
using PubQuizBackend.Models;
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

        [HttpPost("Register")]
        public async Task Register(RegisterUserDto user)
        {
            await _dbContext.Users.AddAsync(user.ToUser());
            await _dbContext.SaveChangesAsync();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginUserDto login)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Email == login.Identifier || x.Username == login.Identifier);

            if (user == null)
                return BadRequest("Invalid username or email.");

            if (user.Password != login.Password)
                return Unauthorized("Invalid password.");

            var accessToken = _jwtService.GenerateAccessToken(user.Id.ToString(), user.Username, CustomConverter.GetStringRole(user.Role));
            var refreshToken = _jwtService.GenerateRefreshToken();

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
