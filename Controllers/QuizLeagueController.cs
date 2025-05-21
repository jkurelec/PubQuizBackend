using Microsoft.AspNetCore.Mvc;
using PubQuizBackend.Model.Dto.QuizLeagueDto;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Util.Extension;

namespace PubQuizBackend.Controllers
{
    [Route("league")]
    [ApiController]
    public class QuizLeagueController : ControllerBase
    {
        private readonly IQuizLeagueService _service;

        public QuizLeagueController(IQuizLeagueService service)
        {
            _service = service;
        }

        [HttpGet("quiz/{id}")]
        public async Task<IActionResult> GetByQuizId(int id)
        {
            return Ok(await _service.GetByQuizId(id)); 
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _service.GetById(id));
        }

        [HttpPost]
        public async Task<IActionResult> Add(NewQuizLeagueDto leagueDto)
        {
            return Ok(await _service.Add(leagueDto, User.GetUserId()));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(NewQuizLeagueDto leagueDto)
        {
            return Ok(await _service.Update(leagueDto, User.GetUserId()));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _service.Delete(id, User.GetUserId()));
        }
    }
}
