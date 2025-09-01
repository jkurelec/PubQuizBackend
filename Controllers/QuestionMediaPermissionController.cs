using Microsoft.AspNetCore.Mvc;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Service.Interface;

namespace PubQuizBackend.Controllers
{
    [Route("permission")]
    [ApiController]
    public class QuestionMediaPermissionController : ControllerBase
    {
        private readonly IQuestionMediaPermissionService _service;
        private readonly string _apiKey;

        public QuestionMediaPermissionController(IQuestionMediaPermissionService service, IConfiguration configuration)
        {
            _service = service;
            _apiKey = configuration["MediaServerApiKey"]
                ?? throw new NotFoundException("Key not found!");
        }

        [HttpGet]
        public IActionResult GetPermissions()
        {
            if (!Request.Headers.TryGetValue("X-API-KEY", out var extractedApiKey))
            {
                return Unauthorized("API key missing.");
            }

            var configuredApiKey = _apiKey;

            if (extractedApiKey != configuredApiKey)
            {
                return Forbid("Invalid API key.");
            }

            return Ok (_service.GetPermissions());
        }
    }
}
