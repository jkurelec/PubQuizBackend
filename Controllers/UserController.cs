using Microsoft.AspNetCore.Mvc;
using PubQuizBackend.Model.Dto.UserDto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;

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

        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            return ["", ""];
        }

        [HttpGet("{id}")]
        public async Task<UserBriefDto?> Get(int id) =>
            new(await _service.GetById(id));

        [HttpPost]
        public async Task<UserDto?> Add(RegisterUserDto userDto)
        {
            return new(await _service.Add(userDto));
        }

        [HttpPut("{id}")]
        public async Task<UserDto?> Update(UserDto userDto)
        {
            return new(await _service.Update(userDto));
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id)
        {
            return await _service.Remove(id);
        }
    }
}
