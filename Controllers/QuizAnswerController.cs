using Microsoft.AspNetCore.Mvc;
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
            return Ok(await _quizAnswerService.GetTeamAnswers(editionResultId, User.GetUserId()));
        }

        [HttpGet("is-detailed/{editionResultId}")]
        public async Task<IActionResult> IsDetailedResult(int roundResultId)
        {
            return Ok(await _quizAnswerService.IsDetailedResult(roundResultId));
        }

        [HttpGet("{editionId}")]
        public async Task<IActionResult> GetEditionResults(int editionId, [FromQuery] bool detailed = false)
        {
            if (detailed)
                return Ok(await _quizAnswerService.GetEditionResultsDetailed(editionId, User.GetUserId()));

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

        [HttpPost("grade")]
        public async Task<IActionResult> GradeTeamAnswers(NewQuizRoundResultDto roundDto)
        {
            return Ok(await _quizAnswerService.GradeTeamAnswers(roundDto, User.GetUserId()));
        }

        [HttpPost("grade/existing")]
        public async Task<IActionResult> GradeExistingTeamAnswers(QuizRoundResultDetailedDto roundDto)
        {
            return Ok(await _quizAnswerService.GradeExistingTeamAnswers(roundDto, User.GetUserId()));
        }

        [HttpPost("round/points")]
        public async Task<IActionResult> AddTeamRoundPoints(NewQuizRoundResultDto roundDto, [FromQuery] bool detailed = false)
        {
            if (!detailed)
                return Ok(await _quizAnswerService.AddTeamRoundPoints(roundDto, User.GetUserId()));

            return Ok(await _quizAnswerService.AddTeamRoundPointsDetailed(roundDto, User.GetUserId()));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnswerPoints(QuizAnswerDetailedDto answerDto)
        {
            return Ok( await _quizAnswerService.UpdateAnswer(answerDto, User.GetUserId()));
        }

        [HttpPut("points/{id}")]
        public async Task<IActionResult> UpdateTeamRoundPoints(NewQuizRoundResultDto answerDto)
        {
            return Ok(await _quizAnswerService.UpdateTeamRoundPoints(answerDto, User.GetUserId()));
        }

        [HttpPut("points/detailed/{id}")]
        public async Task<IActionResult> UpdateTeamRoundPointsDetailed(QuizRoundResultDetailedDto answerDto)
        {
            return Ok(await _quizAnswerService.UpdateTeamRoundPointsDetailed(answerDto, User.GetUserId()));
        }

        [HttpDelete("segment/{roundResultId}")]
        public async Task<IActionResult> DeleteRoundResultSegments(int roundResultId)
        {
            await _quizAnswerService.DeleteRoundResultSegments(roundResultId, User.GetUserId());

            return Ok();
        }
    }
}
