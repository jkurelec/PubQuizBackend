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
        public async Task<IActionResult> GetById(int id, [FromQuery] int detailed = 0)
        {
            if (detailed == 0)
                return Ok(await _service.GetBriefById(id));
            return Ok(await _service.GetDetailedById(id));
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

        [HttpPatch("close/{leagueId}")]
        public async Task<IActionResult> FinishLeague(int leagueId)
        {
            return Ok(await _service.FinishLeague(leagueId, User.GetUserId()));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _service.Delete(id, User.GetUserId()));
        }
    }
}
