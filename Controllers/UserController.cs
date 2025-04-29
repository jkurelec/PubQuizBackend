using Microsoft.AspNetCore.Mvc;
using PubQuizBackend.Models;
using PubQuizBackend.Models.DbModels;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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

        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<User> Get(int id)
        {
            return await _dbContext.Users.FindAsync(1);
        }

        // POST api/<UserController>
        [HttpPost]
        public async Task Post([FromBody]string value)
        {
            await _dbContext.Users.AddAsync(
                new User
                {
                    Username = "test",
                    Password = "test",
                    Role = 1,
                    Firstname = "test",
                    Lastname = "test",
                    Email = "test@test.test",
                }
                );
            await _dbContext.SaveChangesAsync();
            var user = await _dbContext.Users.FindAsync(1);
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
