using Microsoft.AspNetCore.Mvc;
using PubQuizBackend.Model.Dto.ApplicationDto;
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

        [HttpPost("invite")]
        public async Task<IActionResult> InviteUser(TeamMemberDto teamMember)
        {
            await _service.InviteUser(teamMember, User.GetUserId());

            return Ok();
        }

        [HttpPost("apply")]
        public async Task<IActionResult> ApplyToTeam(TeamMemberDto teamMember)
        {
            await _service.ApplyToTeam(teamMember.TeamId, User.GetUserId());

            return Ok();
        }

        [HttpPost("reply/application")]
        public async Task<IActionResult> AnswerApplication(ApplicationResponseDto applicationResponse)
        {
            await _service.AnswerApplication(
                applicationResponse.ApplicationId,
                User.GetUserId(),
                applicationResponse.Response
            );

            return Ok();
        }

        [HttpPost("reply/invitation")]
        public async Task<IActionResult> AnswerInvitation(ApplicationResponseDto applicationResponse)
        {
            await _service.AnswerInvitation(
                applicationResponse.ApplicationId,
                User.GetUserId(),
                applicationResponse.Response
            );

            return Ok ();
        }

        [HttpGet("applications")]
        public async Task<IActionResult> GetApplications()
        {
            return Ok (await _service.GetTeamApplications(User.GetUserId()));
        }

        [HttpGet("invitations")]
        public async Task<IActionResult> GetInvitations()
        {
            return Ok (await _service.GetTeamInvitations(User.GetUserId()));
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

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetByUserId(int id)
        {
            return Ok(await _service.GetByUserId(id));
        }
        

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _service.GetById(id));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(UpdateTeamDto teamDto)
        {
            return Ok (await _service.Update(teamDto, User.GetUserId()));
        }

        [HttpGet("registerRights/{editionId}")]
        public async Task<IActionResult> GetTeamsForRegistration(int editionId)
        {
            return Ok (await _service.GetTeamsForRegistration(User.GetUserId(), editionId));
        }

        [HttpGet("can-invite/{invitee}")]
        public async Task<IActionResult> CanInviteUser(int invitee)
        {
            return Ok(await _service.CanInviteUser(User.GetUserId(), invitee));
        }

        [HttpDelete("leave/{teamId}")]
        public async Task<IActionResult> LeaveTeam(int teamId)
        {
            await _service.LeaveTeam(User.GetUserId(), teamId);

            return NoContent();
        }
    }
}
