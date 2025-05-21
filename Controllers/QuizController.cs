using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NuGet.Protocol.Core.Types;
using PubQuizBackend.Model.Dto.QuizDto;
using PubQuizBackend.Service.Implementation;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Util.Extension;

namespace PubQuizBackend.Controllers
{
    [Route("quiz")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _service;

        public QuizController(IQuizService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int detailed)
        {
            return Ok(await _service.GetAll(detailed == 1));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] int detailed)
        {
            return Ok(await _service.GetById(id, detailed == 1));
        }

        [HttpPost]
        public async Task<IActionResult> Add(NewQuizDto quizDto)
        {
            var newQuiz = await _service.Add(quizDto, User.GetUserId());

            return CreatedAtAction(
                nameof(GetById),
                new { id = newQuiz.Id },
                newQuiz
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(NewQuizDto quizDto)
        {
            return Ok(await _service.Update(quizDto, User.GetUserId()));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.Delete(id, User.GetUserId());
            return Ok();
        }
    }
}
