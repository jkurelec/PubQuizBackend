using Microsoft.AspNetCore.Mvc;
using PubQuizBackend.Enums;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model.Dto.UserDto;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Util.Extension;

namespace PubQuizBackend.Controllers
{
    [Route("user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IRatingHistoryService _ratingHistoryService;

        public UserController(IUserService service, IRatingHistoryService ratingHistoryService)
        {
            _service = service;
            _ratingHistoryService = ratingHistoryService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAll()
        {
            var users = await _service.GetAll();

            return Ok(users.Select(x => new UserBriefDto(x)));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) =>
            Ok(
                new UserBriefDto(await _service.GetById(id))
            );

        //[HttpPost]
        //public async Task<IActionResult> Add(RegisterUserDto userDto)
        //{
        //    var user = await _service.Add(userDto);

        //    return CreatedAtAction(
        //        nameof(Get),
        //        new { id = user.Id },
        //        user
        //    );
        //}

        [HttpPut]
        public async Task<IActionResult> Update(UserDto userDto)
        {
            return (User.GetUserId() == userDto.Id || User.GetUserRole() == Role.ADMIN)
                ? Ok(await _service.Update(userDto))
                : throw new ForbiddenException();

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.Remove(id);

            return Ok();
        }

        [HttpGet("detailed/{id}")]
        public async Task<IActionResult> GetDetailedById(int id)
        {
            return Ok (await _service.GetDetailedById(id));
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string? username = null, string? sortBy = null, bool descending = false, int limit = 50)
        {
            return Ok(await _service.Search(username, sortBy, descending, limit));
        }

        [HttpGet("rating-history/{id}/{timePeriod}")]
        public async Task<IActionResult> GetRatingHistory(int id, TimePeriod timePeriod)
        {
            return Ok(await _ratingHistoryService.GetUserRatingHistories(id, timePeriod));
        }
    }
}
