using Microsoft.AspNetCore.Mvc;
using PubQuizBackend.Util;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PubQuizBackend.Controllers
{
    [Route("test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly PythonServerClient _pythonClient;

        public TestController(PythonServerClient pythonClient)
        {
            _pythonClient = pythonClient;
        }

        [HttpGet]
        public async Task<string?> Get()
        {
            return await _pythonClient.CreateRecommendations(1).ContinueWith(x => x.Result?.Message);
        }

        // GET api/<TestController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<TestController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<TestController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TestController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
