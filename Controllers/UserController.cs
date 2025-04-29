using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Models;
using PubQuizBackend.Models.DbModels;
using PubQuizBackend.Models.Dto;

namespace PubQuizBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly PubQuizContext _dbContext;

        public UserController(PubQuizContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<User> Get(int id) =>
            await _dbContext.Users.FindAsync(1);


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
