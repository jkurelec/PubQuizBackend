using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Controllers
{
    [Route("user")]
    [ApiController]
    [Authorize]
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

        [HttpGet("{id}")]
        public async Task<User?> Get(int id) =>
            await _dbContext.Users.FindAsync(id);


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
