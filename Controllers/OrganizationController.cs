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
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _service;

        public OrganizationController(IOrganizationService service)
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

        [HttpPost]
        public async Task<IActionResult> Add(NewOrganizationDto newOraganizer)
        {
            if (User.GetUserId() != newOraganizer.OwnerId)
                throw new UnauthorizedException();

            var organizer = await _service.Add(newOraganizer);

            return CreatedAtAction(
                nameof(GetById),
                new { id = organizer.Id },
                organizer
            );
        }

        [HttpPost("host")]
        public async Task<IActionResult> AddHost(NewHostDto newHost)
        {
            await OwnerCheck(newHost.OrganizerId);

            var host = await _service.AddHost(newHost);

            return CreatedAtAction(
                nameof(GetById),
                new { id = host.UserBrief.Id },
                host
            );
        }

        [HttpPut]
        public async Task<IActionResult> Update(OrganizationUpdateDto updatedOrganizer)
        {
            await OwnerCheck(updatedOrganizer.Id);

            return Ok(await _service.Update(updatedOrganizer));
        }

        [HttpPut("host")]
        public async Task<IActionResult> UpdateHost(NewHostDto updatedHost)
        {
            await OwnerCheck(updatedHost.OrganizerId);

            return Ok(
                await _service.UpdateHost(updatedHost.OrganizerId, updatedHost.HostId, updatedHost.QuizId, updatedHost.HostPermissions)
            );
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await OwnerCheck(id);

            await _service.Delete(id);

            return NoContent();
        }

        [HttpDelete("{organizerId}/host/{hostId}")]
        public async Task<IActionResult> DeleteHost(int organizerId, int hostId)
        {
            await OwnerCheck(organizerId);

            await _service.DeleteHost(organizerId, hostId);

            return NoContent();
        }

        [HttpDelete("{organizerId}/host/{hostId}/quiz/{quizId}")]
        public async Task<IActionResult> RemoveHostFromQuiz(int organizerId, int hostId, int quizId)
        {
            await OwnerCheck(organizerId);

            await _service.RemoveHostFromQuiz(organizerId, hostId, quizId);

            return NoContent();
        }

        private async Task OwnerCheck(int organizerId)
        {
            var organization = await _service.GetById(organizerId);

            if (User.GetUserId() != organization.Owner.Id)
                throw new UnauthorizedException();
        }
    }
}
