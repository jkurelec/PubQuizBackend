using Microsoft.AspNetCore.Mvc;
using PubQuizBackend.Model.Dto.LocationDto;
using PubQuizBackend.Service.Interface;

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

        [HttpGet("check")]
        public async Task<IActionResult> CheckIfExists(string? locationName = null, string? address = null, string? city = null, string? country = null)
        {
            return Ok(
                await _locationService.CheckIfExists(locationName, address, city, country)
            );
        }

        [HttpGet("new")]
        public async Task<IActionResult> FindNew(string? locationName = null, string? address = null, string? city = null, string? country = null, int limit = 1)
        {
            return Ok(
                await _locationService.FindNew(locationName, address, city, country, limit)
            );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(
                await _locationService.GetById(id)
            );
        }

        [HttpPost]
        public async Task<IActionResult> Add(LocationDetailedDto location)
        {
            var newlocation = await _locationService.Add(location.Name, location.Address, location.City, location.Country);

            return CreatedAtAction(
                nameof(GetById),
                new { id = newlocation.Id },
                newlocation
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(LocationUpdateDto updatedLocation)
        {
            return Ok(
                await _locationService.Update(updatedLocation)
            );
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _locationService.Delete(id);

            return Ok();
        }

        [HttpGet("list")]
        public async Task<IActionResult> Search(string? search = null, int limit = 10)
        {
            var results = string.IsNullOrWhiteSpace(search)
                ? await _locationService.GetAll()
                : await _locationService.SearchByText(search, limit);

            return Ok(results);
        }
    }
}
