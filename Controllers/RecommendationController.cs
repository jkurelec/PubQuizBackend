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

        [HttpGet("user-param")]
        public async Task<IActionResult> UserParam()
        {
            return Ok(await _service.GetUserRecommendationParams(User.GetUserId()));
        }
    }
}
