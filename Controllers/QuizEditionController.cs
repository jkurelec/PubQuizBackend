using Microsoft.AspNetCore.Mvc;
using PubQuizBackend.Enums;
using PubQuizBackend.Model.Dto.QuizEditionDto;
using PubQuizBackend.Model.Dto.QuizQuestionsDto.Basic;
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
            try
            {
                var userId = User.GetUserId();

                return Ok(await _service.GetById(id, userId));
            }
            catch
            {
                return Ok(await _service.GetById(id));
            }
            
        }

        [HttpGet("quiz/{id}")]
        public async Task<IActionResult> GetByQuizId(int id)
        {
            return Ok(await _service.GetByQuizId(id));
        }

        [HttpGet("paged/upcoming")]
        public async Task<IActionResult> GetUpcomingPaged(int page = 1, int pageSize = 10, EditionFilter filter = EditionFilter.NEWEST)
        {
            var items = await _service.GetUpcomingCompletedPage(page, pageSize, filter, upcoming: true);
            var totalCount = await _service.GetTotalCount(EditionTimeFilter.UPCOMING);

            Response.Headers["X-Total-Count"] = totalCount.ToString();

            return Ok(items);
        }

        [HttpGet("paged/completed")]
        public async Task<IActionResult> GetCompletedPaged(int page = 1, int pageSize = 10, EditionFilter filter = EditionFilter.NEWEST)
        {
            var items = await _service.GetUpcomingCompletedPage(page, pageSize, filter, upcoming: false);
            var totalCount = await _service.GetTotalCount(EditionTimeFilter.PAST);

            Response.Headers["X-Total-Count"] = totalCount.ToString();

            return Ok(items);
        }

        [HttpGet("paged/all")]
        public async Task<IActionResult> GetAllPaged(int page = 1, int pageSize = 10, EditionFilter filter = EditionFilter.NEWEST)
        {
            var items = await _service.GetPage(page, pageSize, filter);
            var totalCount = await _service.GetTotalCount(EditionTimeFilter.ALL);

            Response.Headers["X-Total-Count"] = totalCount.ToString();

            return Ok(items);
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

        [HttpGet("location/{id}")]
        public async Task<IActionResult> GetByLocationId(int id)
        {
            return Ok(await _service.GetByLocationId(id));
        }

        [HttpPost("profile-image/{editionId}")]
        public async Task<IActionResult> UpdateProfileImage(IFormFile image, int editionId)
        {
            var newImageName = await _service.UpdateProfileImage(image, editionId, User.GetUserId());

            return Ok(newImageName);
        }

        [HttpGet("detailed-questions/{editionId}")]
        public async Task<IActionResult> HasDetailedQuestions(int editionId)
        {
            var result = await _service.HasDetailedQuestions(editionId);

            return Ok(new DetailedQuestionStatusDto { Value = result });
        }

        [HttpPatch("detailed-questions/{editionId}")]
        public async Task<IActionResult> SetDetailedQuestions(int editionId, [FromBody] DetailedQuestionStatusDto dto)
        {
            if (dto.Value is null)
                return BadRequest("Value cannot be null.");

            await _service.SetDetailedQuestions(editionId, User.GetUserId(), dto.Value.Value);
            return Ok();
        }

        [HttpGet("team/{teamId}")]
        public async Task<IActionResult> GetByTeamId(int teamId)
        {
            return Ok(await _service.GetByTeamId(teamId));
        }
    }
}
