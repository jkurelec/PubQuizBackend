using Microsoft.AspNetCore.Mvc;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model.Dto.ApplicationDto;
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

        [HttpGet("host/{hostId}/quiz/{quizId}")]
        public async Task<IActionResult> GetHost(int hostId, int quizId)
        {
            return Ok(await _service.GetHost(hostId, quizId));
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
                throw new ForbiddenException();

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

        [HttpPost("profile-image")]
        public async Task<IActionResult> UpdateProfileImage(IFormFile image)
        {
            var newFileName = await _service.UpdateProfileImage(User.GetUserId(), image);

            return Ok(newFileName);
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

        [HttpGet("invite")]
        public async Task<IActionResult> GetInvitations()
        {
            return Ok(await _service.GetInvitations(User.GetUserId()));
        }

        [HttpPost("invite/send")]
        public async Task<IActionResult> InviteHostToOrganization(QuizInvitationRequestDto request)
        {
            await _service.InviteHostToOrganization(request, User.GetUserId());

            return Ok();
        }

        [HttpPost("invite/respond")]
        public async Task<IActionResult> RespondToInvitation(ApplicationResponseDto response)
        {
            await _service.RespondToInvitation(User.GetUserId(), response);

            return Ok();
        }

        [HttpGet("host")]
        public async Task<IActionResult> GetByHost()
        {
            return Ok(await _service.GetByHost(User.GetUserId()));
        }

        [HttpGet("from-owner")]
        public async Task<IActionResult> GetOwnerOrganization()
        {
            return Ok(await _service.GetOwnerOrganization(User.GetUserId()));
        }

        [HttpGet("for-new-host/{hostId}")]
        public async Task<IActionResult> GetAvaliableQuizzesForNewHost(int hostId)
        {
            var organization = await _service.GetOwnerOrganization(User.GetUserId())
                ?? throw new ForbiddenException();

            return Ok(await _service.GetAvaliableQuizzesForNewHost(hostId, organization.Id));
        }

        [HttpGet("invitation/pending")]
        public async Task<IActionResult> GetOrganizationPendingQuizInvitations()
        {
            var organization = await _service.GetOwnerOrganization(User.GetUserId())
                ?? throw new ForbiddenException();

            return Ok(await _service.GetOrganizationPendingQuizInvitations(organization.Id));
        }

        [HttpGet("hosts-by-quiz/{quizId}")]
        public async Task<IActionResult> GetHostsByQuiz(int quizId)
        {
            return Ok(await _service.GetHostsByQuiz(quizId));
        }

        private async Task OwnerCheck(int organizerId)
        {
            var organization = await _service.GetById(organizerId);

            if (User.GetUserId() != organization.Owner.Id)
                throw new ForbiddenException();
        }
    }
}
