using Microsoft.AspNetCore.Mvc;
using PubQuizBackend.Enums;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model.Dto.QuizAnswerDto;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Util;
using PubQuizBackend.Util.Extension;
using System.Collections.Generic;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PubQuizBackend.Controllers
{
    [Route("answer")]
    [ApiController]
    public class QuizAnswerController : ControllerBase
    {
        private readonly IQuizAnswerService _quizAnswerService;
        private readonly PythonServerClient _pythonServerClient;

        public QuizAnswerController(IQuizAnswerService quizAnswerService, PythonServerClient pythonServerClient)
        {
            _quizAnswerService = quizAnswerService;
            _pythonServerClient = pythonServerClient;
        }

        [HttpGet("teamAnswer/{editionResultId}")]
        public async Task<IActionResult> GetTeamAnswers(int editionResultId)
        {
            return Ok(await _quizAnswerService.GetTeamAnswers(editionResultId, User.GetUserId()));
        }

        [HttpGet("is-detailed/{roundResultId}")]
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

        [HttpPost("auto-grade")]
        public async Task<IActionResult> AutoGradeTeamAnswers([FromForm] IFormFile image, [FromForm] string roundResult)
        {
            if (image == null || image.Length == 0)
                throw new BadRequestException("No image uploaded.");

            if (User.GetUserRole() == Role.ATTENDEE)
                throw new UnauthorizedException();

            var result = await _pythonServerClient.ExtractSentencesFromImage(image);

            var data = JsonSerializer.Deserialize<List<PredictedAnswers>>(result);

            var dto = JsonSerializer.Deserialize<QuizRoundResultDetailedDto>(roundResult);

            var round = await _quizAnswerService.AutogradeAnswers(dto, data);

            return Ok(round);
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

        public class PredictedAnswers
        {
            public string number { get; set; }
            public string answer { get; set; }
        }
    }
}
