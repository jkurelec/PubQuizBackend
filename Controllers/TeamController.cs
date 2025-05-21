using Microsoft.AspNetCore.Mvc;
using PubQuizBackend.Model.Dto.TeamDto;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Util.Extension;

namespace PubQuizBackend.Controllers
{
    [Route("team")]
    [ApiController]
    public class TeamController : BaseController
    {
        private readonly ITeamService _service;

        public TeamController(ITeamService service)
        {
            _service = service;
        }

        [HttpPost("member")]
        public async Task<IActionResult> AddMember(TeamMemberDto teamMember)
        {
            await _service.AddMember(teamMember, User.GetUserId());
            
            return NoContentAtAction(
                nameof(GetById),
                new { id = teamMember.TeamId }
                );
        }

        [HttpPut("owner/{id}")]
        public async Task<IActionResult> ChangeOwner(int id)
        {
            await _service.ChangeOwner(id, User.GetUserId());

            return NoContentAtAction(
                nameof(GetByOwnerId),
                new { id }
                );
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete()
        {
            await _service.Delete(User.GetUserId());

            return NoContent();
        }

        [HttpPut("member/{id}")]
        public async Task<IActionResult> EditMember(TeamMemberDto teamMember)
        {
            await _service.EditMember(teamMember, User.GetUserId());

            return NoContentAtAction(
                nameof(GetById),
                new { teamMember.TeamId }
                );
        }

        [HttpDelete("member/{id}")]
        public async Task<IActionResult> RemoveMember(int id)
        {
            await _service.RemoveMember(id, User.GetUserId());

            return NoContent();
        }

        [HttpGet()]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpPost()]
        public async Task<IActionResult> Add(NewTeamDto teamDto)
        {
            return Ok(await _service.Add(teamDto.Name, User.GetUserId()));
        }

        [HttpGet("owner/{id}")]
        public async Task<IActionResult> GetByOwnerId(int id)
        {
            return Ok(await _service.GetByOwnerId(id));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _service.GetById(id));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(UpdateTeamDto teamDto)
        {
            return Ok(await _service.Update(teamDto, User.GetUserId()));
        }
    }
}
