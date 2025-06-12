using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using PubQuizBackend.Model.Dto.QuizAnswerDto;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Util.Extension;

namespace PubQuizBackend.Controllers
{
    [Route("answer")]
    [ApiController]
    public class QuizAnswerController : ControllerBase
    {
        private readonly IQuizAnswerService _quizAnswerService;

        public QuizAnswerController(IQuizAnswerService quizAnswerService)
        {
            _quizAnswerService = quizAnswerService;
        }

        [HttpGet("teamAnswer/{editionResultId}")]
        public async Task<IActionResult> GetTeamAnswers(int editionResultId)
        {
            return Ok (await _quizAnswerService.GetTeamAnswers(editionResultId, User.GetUserId()));
        }

        [HttpGet]
        public async Task<IActionResult> GetEditionResults(int editionId)
        {
            return Ok (await _quizAnswerService.GetEditionResults(editionId, User.GetUserId()));
        }

        [HttpPut("rank/{editionId}")]
        public async Task<IActionResult> RankTeamsOnEdition(int editionId)
        {
            return Ok(await _quizAnswerService.RankTeamsOnEdition(editionId, User.GetUserId()));
        }

        [HttpPut("rank/breaktie/{promotedId}/{editionId}")]
        public async Task<IActionResult> BreakTie(int promotedId, int editionId)
        {
            return Ok(await _quizAnswerService.BreakTie(promotedId, editionId, User.GetUserId()));
        }

        [HttpPost]
        public async Task<IActionResult> GradeTeamAnswers(NewQuizRoundResultDto roundDto)
        {
            return Ok(await _quizAnswerService.GradeTeamAnswers(roundDto, User.GetUserId()));
        }

        [HttpPost("points")]
        public async Task<IActionResult> AddTeamRoundPoints(NewQuizRoundResultDto roundDto)
        {
            return Ok(await _quizAnswerService.AddTeamRoundPoints(roundDto, User.GetUserId()));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnswerPoints(QuizAnswerDetailedDto answerDto)
        {
            return Ok( await _quizAnswerService.UpdateAnswer(answerDto, User.GetUserId()));
        }

        [HttpPut("points/{id}")]
        public async Task<IActionResult> UpdateTeamRoundPoints(QuizAnswerDetailedDto answerDto)
        {
            return Ok(await _quizAnswerService.UpdateAnswer(answerDto, User.GetUserId()));
        }

        //Jel treba delete?
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
