using Microsoft.AspNetCore.Mvc;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.ApplicationDto;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Util.Extension;

namespace PubQuizBackend.Controllers
{
    [Route("application")]
    [ApiController]
    public class QuizEditionApplicationController : ControllerBase
    {
        private readonly IQuizEditionApplicationService _service;

        public QuizEditionApplicationController(IQuizEditionApplicationService service)
        {
            _service = service;
        }

        [HttpDelete("team/{teamId}/{editionId}")]
        public async Task<IActionResult> RemoveTeamFromEdition(int editionId, int teamId)
        {
            await _service.RemoveTeamFromEdition(editionId, teamId, User.GetUserId());

            return NoContent();
        }

        [HttpDelete("withdraw/{teamId}/{editionId}")]
        public async Task<IActionResult> WithdrawFromEdition(int editionId, int teamId)
        {
            await _service.WithdrawFromEdition(editionId, teamId, User.GetUserId());

            return NoContent();
        }

        [HttpPost("apply")]
        public async Task<IActionResult> Apply(QuizEditionApplicationRequestDto application)
        {
            await _service.ApplyTeam(application, User.GetUserId());

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetApplications(int id, [FromQuery] bool unanswered = true)
        {
            return Ok(await _service.GetApplications(id, User.GetUserId(), unanswered));
        }

        [HttpPost("respond")]
        public async Task<IActionResult> RespondToApplication(QuizEditionApplicationResponseDto application)
        {
            await _service.RespondToApplication(application, User.GetUserId());

            return NoContent();
        }

        [HttpGet("accepted/{id}")]
        public async Task<IActionResult> GetAcceptedApplicationsByEdition(int id)
        {
            return Ok (await _service.GetAcceptedApplicationsByEdition(id));
        }

        [HttpGet("check/{editionId}")]
        public async Task<IActionResult> CheckIfUserApplied(int editionId)
        {
            //int userId;

            //try
            //{
            //    userId = User.GetUserId();
            //}
            //catch
            //{
            //    return Ok (false);
            //}

            return Ok (await _service.CheckIfUserApplied(User.GetUserId(), editionId));
        }
    }
}
