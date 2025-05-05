using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto;
using PubQuizBackend.Repository.Interface;
using System.Globalization;

namespace PubQuizBackend.Repository.Implementation
{
    public class LocationRepository : ILocationRepository
    {
        private readonly PubQuizContext _dbContext;
        private readonly IPostalCodeRepository _postalCodeRepository;

        public LocationRepository(PubQuizContext dbContext, ICityRepository cityRepository, IPostalCodeRepository postalCodeRepository)
        {
            _dbContext = dbContext;
            _postalCodeRepository = postalCodeRepository;
        }

        public async Task<LocationDetailsDto> Add(string? locationName = null, string? address = null, string? city = null, string? country = null, int limit = 1)
        {
            var locationDto = await CheckIfExists(locationName, address, city, country, limit);

            if (locationDto != null)
                return locationDto;

            locationDto = await FindNew(locationName, address, city, country, limit).ContinueWith(x => x.Result[0]);

            PostalCode? postalCode = await _postalCodeRepository.GetPostalCodeByCode(locationDto.PostalCode);


            if (postalCode == null)
                postalCode = await _postalCodeRepository.AddPostalCode(
                    new ()
                    {
                        Code = locationDto.PostalCode,
                        City = new()
                        {
                            Name = locationDto.City,
                            Country = new()
                            {
                                Name = locationDto.Country,
                                CountryCode = locationDto.CountryCode
                            }
                        }
                    }
                );

            locationDto.PostalCodeId = postalCode.Id;
            locationDto.CityId = postalCode.CityId;

            var location = locationDto.ToLocation(postalCode);



            await _dbContext.Locations.AddAsync(location);
            await _dbContext.SaveChangesAsync();

            return locationDto;
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var location = await _dbContext.Locations.FindAsync(id);
                _dbContext.Locations.Remove(location);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Location>> GetAllLocations()
        {
            return await _dbContext.Locations.ToListAsync();
        }

        public async Task<Location?> GetLocationById(int id)
        {
            return await _dbContext.Locations.FindAsync(id);
        }

        public async Task<Location?> GetLocationByName(string name)
        {
            return await _dbContext.Locations.Where(x => x.Name == name).FirstOrDefaultAsync();
        }

        public async Task<List<Location>> GetLocationsByCityId(int id)
        {
            return await _dbContext.Locations.Where(x => x.CityId == id).ToListAsync();
        }

        public async Task<bool> Update(Location location)
        {
            _dbContext.Entry(location).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return false; ;
        }

        public async Task<List<LocationDetailsDto>?> FindNew(string? locationName = null, string? address = null, string? city = null, string? country = null, int limit = 1)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("PubQuiz/v1.0 (kurelec81@gmail.com)");

            var url = $"https://nominatim.openstreetmap.org/search?amenity={Uri.EscapeDataString(locationName ?? "")}&street={Uri.EscapeDataString(address ?? "")}&city={Uri.EscapeDataString(city ?? "")}&country={Uri.EscapeDataString(country ?? "")}&format=jsonv2&addressdetails=1&limit={limit}";
            try {
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error: Status code {(int)response.StatusCode}");
                    return null!;
                }

                var responseBody = await response.Content.ReadAsStringAsync();

                dynamic results = JsonConvert.DeserializeObject(responseBody);

                if (results.Count == 0)
                {
                    Console.WriteLine("No results found.");
                    return null!;
                }

                var list = new List<LocationDetailsDto>();

                for(int i = 0; i< results.Count; i++)
                {
                    JObject osmAddressObj = results[i].address;

                    list.Add(
                        new ()
                        {
                            Name = osmAddressObj.Value<string>("amenity"),
                            Address = $"{osmAddressObj.Value<string>("road")} {osmAddressObj.Value<string>("house_number")}",
                            PostalCode = osmAddressObj.Value<string>("postcode"),
                            City = osmAddressObj.Value<string>("city"),
                            Country = osmAddressObj.Value<string>("country"),
                            CountryCode = osmAddressObj.Value<string>("country_code"),
                            Lat = double.Parse((string)results[i]["lat"], CultureInfo.InvariantCulture),
                            Lon = double.Parse((string)results[i]["lon"], CultureInfo.InvariantCulture),
                        }
                    );
                }

                return list;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP request failed: {ex.Message}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Failed to parse JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }

            return null!;
        }

        public async Task<LocationDetailsDto?> CheckIfExists(string? locationName = null, string? address = null, string? city = null, string? country = null, int limit = 1)
        {
            var query = _dbContext.Locations
                .Include(l => l.City)
                .ThenInclude(c => c.Country)
                .Include(l => l.PostalCode)
                .ThenInclude(pc => pc.City)
                .ThenInclude(c => c.Country)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(locationName))
                query = query.Where(l => EF.Functions.Like(l.Name.ToLower(), $"%{locationName.ToLower()}%"));

            if (!string.IsNullOrWhiteSpace(address))
            {
                var addressTokens = address.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var token in addressTokens)
                    query = query.Where(l => EF.Functions.Like(l.Address.ToLower(), $"%{token}%"));
            }

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(l => EF.Functions.Like(l.PostalCode!.City!.Name.ToLower(), $"%{city.ToLower()}%"));

            if (!string.IsNullOrWhiteSpace(country))
                query = query.Where(l => EF.Functions.Like(l.PostalCode!.City!.Country.Name.ToLower(), $"%{country.ToLower()}%"));

            var location = await query.Take(limit).FirstOrDefaultAsync();

            return location != null
                ? new(location)
                : null;
        }
    }
}
