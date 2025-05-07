using Microsoft.AspNetCore.Mvc;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.LocationDto;
using PubQuizBackend.Service.Interface;
using System.Threading.Tasks;

namespace PubQuizBackend.Controllers
{
    [Route("location")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        public async Task<LocationDetailsDto?> Get(string? locationName = null, string? address = null, string? city = null, string? country = null)
        {
            return await _locationService.CheckIfExists(locationName, address, city, country);
        }

        [HttpGet("new")]
        public async Task<List<LocationDetailsDto>?> FindNew(string? locationName = null, string? address = null, string? city = null, string? country = null, int limit = 1)
        {
            return await _locationService.FindNew(locationName, address, city, country, limit);
        }

        [HttpGet("{id}")]
        public async Task<Location?> Get(int id)
        {
            return await _locationService.GetLocationById(id);
        }

        [HttpPost]
        public async Task<LocationDetailsDto> Post(LocationDetailsDto location)
        {
            return await _locationService.Add(location.Name, location.Address, location.City, location.Country, 1);
        }

        [HttpPut("{id}")]
        public async Task<LocationDetailsDto> Update(LocationUpdateDto updatedLocation)
        {
            return new(await _locationService.Update(updatedLocation));
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id)
        {
            return await _locationService.Delete(id);
        }
    }
}
