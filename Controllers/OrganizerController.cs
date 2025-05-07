using Microsoft.AspNetCore.Mvc;
using PubQuizBackend.Model.Dto.OrganizerDto;
using PubQuizBackend.Service.Interface;

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
        public async Task<OrganizerDto?> Get(int id)
        {
            return await _service.GetById(id);
        }

        [HttpGet("{organizerId}/host/{hostId}")]
        public async Task<HostDto?> GetHost(int organizerId, int hostId)
        {
            return await _service.GetHost(organizerId, hostId);
        }

        [HttpGet("hosts/{organizerId}")]
        public async Task<List<HostDto>?> GetHostsFromOrganization(int organizerId)
        {
            return await _service.GetHostsFromOrganization(organizerId);
        }

        [HttpPost("add")]
        public async Task<OrganizerDto?> Add(NewOrganizerDto newOraganizer)
        {
            return await _service.Add(newOraganizer.Name, newOraganizer.OwnerId);
        }

        [HttpPost("host/add")]
        public async Task<HostDto?> AddHost(NewHostDto newHost)
        {
            return await _service.AddHost(newHost.OrganizerId, newHost.HostId, newHost.HostPermissions);
        }

        [HttpPut]
        public async Task<OrganizerDto?> Update(OrganizerUpdateDto updatedOrganizer)
        {
            return await _service.Update(updatedOrganizer);
        }

        [HttpPut("host")]
        public async Task<HostDto?> UpdateHost(NewHostDto updatedHost)
        {
            return await _service.UpdateHost(updatedHost.OrganizerId, updatedHost.HostId, updatedHost.HostPermissions);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id)
        {
            return await _service.Delete(id);
        }

        [HttpDelete("{organizerId}/host/{hostId}")]
        public async Task<bool> DeleteHost(int organizerId, int hostId)
        {
            return await _service.DeleteHost(organizerId, hostId);
        }
    }
}
