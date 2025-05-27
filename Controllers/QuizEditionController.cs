using Microsoft.AspNetCore.Mvc;
using PubQuizBackend.Model.Dto.ApplicationDto;
using PubQuizBackend.Model.Dto.QuizEditionDto;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Util.Extension;

namespace PubQuizBackend.Controllers
{
    [Route("edition")]
    [ApiController]
    public class QuizEditionController : ControllerBase
    {
        private readonly IQuizEditionService _service;

        public QuizEditionController(IQuizEditionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _service.GetById(id));
        }

        [HttpGet("quiz/{id}")]
        public async Task<IActionResult> GetByQuizId(int id)
        {
            return Ok(await _service.GetByQuizId(id));
        }

        [HttpPost]
        public async Task<IActionResult> Add(NewQuizEditionDto editionDto)
        {
            var newEdition = await _service.Add(editionDto, User.GetUserId());

            return CreatedAtAction(
                nameof(GetById),
                new { id = newEdition.Id },
                newEdition
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(NewQuizEditionDto editionDto)
        {
            return Ok(await _service.Update(editionDto, User.GetUserId()));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.Delete(id, User.GetUserId());

            return NoContent();
        }

        [HttpPost("apply")]
        public async Task<IActionResult> Apply(QuizEditionApplicationRequestDto application)
        {
            await _service.ApplyTeam(application, User.GetUserId());

            return NoContent();
        }

        [HttpGet("application/{id}")]
        public async Task<IActionResult> GetApplications(int id, [FromQuery] bool unanswered = true)
        {
            return Ok(await _service.GetApplications(id, User.GetUserId(), unanswered));
        }

        [HttpPost("application")]
        public async Task<IActionResult> RespondToApplication(QuizEditionApplicationResponseDto application)
        {
            await _service.RespondToApplication(application, User.GetUserId());

            return NoContent();
        }
    }
}
