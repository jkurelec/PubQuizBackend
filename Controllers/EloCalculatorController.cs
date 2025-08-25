using Microsoft.AspNetCore.Mvc;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Util.Extension;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PubQuizBackend.Controllers
{
    [Route("elo")]
    [ApiController]
    public class EloCalculatorController : ControllerBase
    {
        private readonly IEloCalculatorService _service;

        public EloCalculatorController(IEloCalculatorService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CalculateElo(int editionId)
        {
            await _service.CalculateEditionElo(editionId, User.GetUserId());

            return Ok("Ratings succesfully updated!");
        }

        [HttpGet("{editionId}")]
        public async Task<IActionResult> IsRated(int editionId)
        {
            return Ok(await _service.IsEditionRated(editionId));
        }

        [HttpGet("probability/{editionId}")]
        public async Task<IActionResult> GetTeamProbability(int editionId)
        {
            return Ok(await _service.GetProbability(editionId));
        }
    }
}
