using Microsoft.AspNetCore.Mvc;
using PubQuizBackend.Model.Dto.QuizQuestionsDto.Basic;
using PubQuizBackend.Model.Dto.QuizQuestionsDto.Specific;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Util.Extension;
using System.Text.Json;

namespace PubQuizBackend.Controllers
{
    [Route("upcoming/question")]
    [ApiController]
    public class UpcomingQuizQuestionController : ControllerBase
    {
        private readonly IUpcomingQuizQuestionService _service;

        public UpcomingQuizQuestionController(IUpcomingQuizQuestionService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestion(int id)
        {
            return Ok(await _service.GetQuestion(id, User.GetUserId()));
        }

        [HttpGet("segment/{id}")]
        public async Task<IActionResult> GetSegment(int id)
        {
            return Ok(await _service.GetSegment(id, User.GetUserId()));
        }

        [HttpGet("rounds/{editionId}")]
        public async Task<IActionResult> GetRounds(int editionId, [FromQuery] bool detailed = false)
        {
            return Ok(await _service.GetRounds(editionId, User.GetUserId(), detailed));
        }

        [HttpGet("round/{id}")]
        public async Task<IActionResult> GetRound(int id)
        {
            return Ok(await _service.GetRound(id, User.GetUserId()));
        }

        [HttpGet("has-questions/{editionId}")]
        public async Task<IActionResult> DoesEditionHaveQuestions(int editionId)
        {
            return Ok(await _service.DoesEditionHaveQuestions(editionId, User.GetUserId()));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            await _service.DeleteQuestion(id, User.GetUserId());

            return NoContent();
        }

        [HttpDelete("segment/{id}")]
        public async Task<IActionResult> DeleteSegment(int id)
        {
            await _service.DeleteSegment(id, User.GetUserId());

            return NoContent();
        }

        [HttpDelete("round/{id}")]
        public async Task<IActionResult> DeleteRound(int id)
        {
            await _service.DeleteRound(id, User.GetUserId());

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> AddQuestion([FromForm(Name = "questionDto")] string questionDto, IFormFile? file)
        {
            var dto = JsonSerializer.Deserialize<QuizQuestionDto>(questionDto);

            if (dto == null)
                return BadRequest("Invalid questionDto JSON.");

            var question = await _service.AddQuestion(dto, User.GetUserId(), file);

            return CreatedAtAction(
                nameof(GetQuestion),
                new { id = question.Id },
                question
            );
        }

        [HttpPost("segment")]
        public async Task<IActionResult> AddSegment(QuizSegmentDto segmentDto)
        {
            var segment = await _service.AddSegment(segmentDto, User.GetUserId());

            return CreatedAtAction(
                nameof(GetSegment),
                new { id = segment.Id },
                segment
            );
        }

        [HttpPost("round")]
        public async Task<IActionResult> AddRound(QuizRoundDto roundDto)
        {
            var round = await _service.AddRound(roundDto, User.GetUserId());

            return CreatedAtAction(
                nameof(GetRound),
                new { id = round.Id },
                round
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditQuestion([FromForm(Name = "questionDto")] string questionDto, IFormFile? file)
        {
            var dto = JsonSerializer.Deserialize<QuizQuestionDto>(questionDto);

            if (dto == null)
                return BadRequest("Invalid questionDto JSON.");

            var question = await _service.EditQuestion(dto, User.GetUserId(), file);

            return Ok (question);
        }

        [HttpPut("segment/{id}")]
        public async Task<IActionResult> EditSegment(QuizSegmentDto segmentDto)
        {
            return Ok(await _service.EditSegment(segmentDto, User.GetUserId()));
        }

        [HttpPut("round/{id}")]
        public async Task<IActionResult> EditRound(QuizRoundBriefDto roundDto)
        {
            return Ok(await _service.EditRound(roundDto, User.GetUserId()));
        }

        [HttpPut("order/{id}")]
        public async Task<IActionResult> UpdateQuestionOrder(UpdateOrderDto orderDto)
        {
            return Ok(await _service.UpdateQuestionOrder(orderDto, User.GetUserId()));
        }

        [HttpPut("segment/order/{id}")]
        public async Task<IActionResult> UpdateSegmentOrder(UpdateOrderDto orderDto)
        {
            return Ok(await _service.UpdateSegmentOrder(orderDto, User.GetUserId()));
        }

        [HttpPut("round/order/{id}")]
        public async Task<IActionResult> UpdateRoundOrder(UpdateOrderDto orderDto)
        {
            return Ok(await _service.UpdateRoundOrder(orderDto, User.GetUserId()));
        }
    }
}
