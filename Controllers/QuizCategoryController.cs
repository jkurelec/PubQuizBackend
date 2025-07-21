using Microsoft.AspNetCore.Mvc;
using PubQuizBackend.Auth.RoleAndAudienceFilter;
using PubQuizBackend.Enums;
using PubQuizBackend.Model.Dto.QuizCategoryDto;
using PubQuizBackend.Service.Interface;

namespace PubQuizBackend.Controllers
{
    [Route("category")]
    [ApiController]
    public class QuizCategoryController : ControllerBase
    {
        private readonly IQuizCategoryService _service;

        public QuizCategoryController(IQuizCategoryService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            return Ok(_service.GetById(id));
        }

        [HttpGet("name/{name}")]
        public IActionResult GetByName(string name)
        {
            return Ok(_service.GetByName(name));
        }

        [HttpGet("super/{superCategoryId}")]
        public IActionResult GetBySuperCategoryId(int? superCategoryId)
        {
            return Ok(_service.GetBySuperCategoryId(superCategoryId));
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAll());
        }

        [RoleAndAudience(Role.ORGANIZER, Audience.ORGANIZER)]
        [HttpPost]
        public async Task<IActionResult> Add(QCategoryDto quizCategory)
        {
            var newCategory = await _service.Add(quizCategory);

            return CreatedAtAction(
                nameof(GetById),
                new { id = newCategory.Id },
                newCategory
            );
        }
    }
}
