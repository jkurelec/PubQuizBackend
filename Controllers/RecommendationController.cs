using Microsoft.AspNetCore.Mvc;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Util.Extension;

namespace PubQuizBackend.Controllers
{
    [Route("recommendation")]
    [ApiController]
    public class RecommendationController : ControllerBase
    {
        private readonly IRecommendationService _service;

        public RecommendationController(IRecommendationService service)
        {
            _service = service;
        }

        [HttpGet("feedback-request")]
        public async Task<IActionResult> GetEditionInfoForFeedback()
        {
            return Ok (await _service.GetEditionInfoForFeedback(User.GetUserId()));
        }

        // GET api/<RecommendationController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<RecommendationController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<RecommendationController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<RecommendationController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
