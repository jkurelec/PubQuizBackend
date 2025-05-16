using Microsoft.AspNetCore.Mvc;
using PubQuizBackend.Auth.RoleAndAudienceFilter;
using PubQuizBackend.Enums;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model.Dto.OrganizationDto;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Util.Extension;

namespace PubQuizBackend.Controllers
{
    [Route("organizer")]
    [ApiController]
    public class OrganizerController : ControllerBase
    {
        private readonly IOrganizerService _service;

        public OrganizerController(IOrganizerService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _service.GetById(id));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{organizerId}/host/{hostId}/quiz/{quizId}")]
        public async Task<IActionResult> GetHost(int organizerId, int hostId, int quizId)
        {
            return Ok(await _service.GetHost(organizerId, hostId, quizId));
        }

        [HttpGet("hosts/{organizerId}")]
        public async Task<IActionResult> GetHostsFromOrganization(int organizerId)
        {
            return Ok(await _service.GetHostsFromOrganization(organizerId));
        }

        [RoleAndAudience(Role.ORGANIZER, Audience.ORGANIZER)]
        [HttpPost]
        public async Task<IActionResult> Add(NewOrganizationDto newOraganizer)
        {
            if (User.GetUserId() != newOraganizer.OwnerId)
                throw new UnauthorizedException();

            var organizer = await _service.Add(newOraganizer.Name, newOraganizer.OwnerId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = organizer.Id },
                organizer
            );
        }

        [RoleAndAudience(Role.ORGANIZER, Audience.ORGANIZER)]
        [HttpPost("host")]
        public async Task<IActionResult> AddHost(NewHostDto newHost)
        {
            await OwnerCheck(newHost.OrganizerId);

            var host = await _service.AddHost(newHost.OrganizerId, newHost.HostId, newHost.QuizId, newHost.HostPermissions);

            return CreatedAtAction(
                nameof(GetById),
                new { id = host.UserBrief.Id },
                host
            );
        }

        [RoleAndAudience(Role.ORGANIZER, Audience.ORGANIZER)]
        [HttpPut]
        public async Task<IActionResult> Update(OrganizationUpdateDto updatedOrganizer)
        {
            await OwnerCheck(updatedOrganizer.Id);

            return Ok(await _service.Update(updatedOrganizer));
        }

        [RoleAndAudience(Role.ORGANIZER, Audience.ORGANIZER)]
        [HttpPut("host")]
        public async Task<IActionResult> UpdateHost(NewHostDto updatedHost)
        {
            await OwnerCheck(updatedHost.OrganizerId);

            return Ok(
                await _service.UpdateHost(updatedHost.OrganizerId, updatedHost.HostId, updatedHost.QuizId, updatedHost.HostPermissions)
            );
        }

        [RoleAndAudience(Role.ORGANIZER, Audience.ORGANIZER)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await OwnerCheck(id);

            return Ok(await _service.Delete(id));
        }

        [RoleAndAudience(Role.ORGANIZER, Audience.ORGANIZER)]
        [HttpDelete("{organizerId}/host/{hostId}")]
        public async Task<IActionResult> DeleteHost(int organizerId, int hostId)
        {
            await OwnerCheck(organizerId);

            return Ok(await _service.DeleteHost(organizerId, hostId));
        }

        [RoleAndAudience(Role.ORGANIZER, Audience.ORGANIZER)]
        [HttpDelete("{organizerId}/host/{hostId}/quiz/{quizId}")]
        public async Task<IActionResult> RemoveHostFromQuiz(int organizerId, int hostId, int quizId)
        {
            await OwnerCheck(organizerId);

            return Ok(await _service.RemoveHostFromQuiz(organizerId, hostId, quizId));
        }

        private async Task OwnerCheck(int organizerId)
        {
            var host = await _service.GetById(organizerId);

            if (User.GetUserId() != host.Owner.Id)
                throw new UnauthorizedException();
        }
    }
}
