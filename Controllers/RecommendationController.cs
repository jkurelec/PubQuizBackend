using Microsoft.AspNetCore.Mvc;
using PubQuizBackend.Model.Dto.RecommendationDto;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Util.Extension;

namespace PubQuizBackend.Controllers
{
    [Route("recommendation")]
    [ApiController]
    public class RecommendationController : ControllerBase
    {
        private readonly IRecommendationService _service;

        public RecommendationController(IRecommendationService service)
        {
            _service = service;
        }

        [HttpGet("feedback-request")]
        public async Task<IActionResult> GetEditionInfoForFeedback()
        {
            return Ok (await _service.GetEditionInfoForFeedback(User.GetUserId()));
        }

        [HttpPost("feedback")]
        public async Task<IActionResult> SetFeedback(UserFeedbackDto feedback)
        {
            await _service.SetUserFeedback(feedback, User.GetUserId());

            return Ok();
        }
    }
}
