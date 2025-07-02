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

        public UserController(IUserService userService)
        {
            _service = userService;
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
    }
}
