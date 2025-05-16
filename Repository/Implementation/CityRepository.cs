using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Exceptions;
using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Repository.Interface;

namespace PubQuizBackend.Repository.Implementation
{
    public class CityRepository : ICityRepository
    {
        private readonly PubQuizContext _dbContext;
        private readonly ICountryRepository _countryRepository;

        public CityRepository(PubQuizContext dbContext, ICountryRepository countryRepository)
        {
            _dbContext = dbContext;
            _countryRepository = countryRepository;
        }

        public async Task<City> AddCity(City city)
        {
            var country = await _countryRepository.GetCountryByName(city.Country.Name)
                ?? await _countryRepository.AddCountry(city.Country);

            city.CountryId = country.Id;

            await _dbContext.Cities.AddAsync(city);
            await _dbContext.SaveChangesAsync();

            return city;
        }

        public async Task<bool> DeleteCity(City city)
        {
            _dbContext.Cities.Remove(city);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<City>> GetAllCities()
        {
            return await _dbContext.Cities.ToListAsync();
        }

        public async Task<List<City>> GetCitiesByCountryId(int id)
        {
            var list = await _dbContext.Cities.Where(x => x.CountryId == id).ToListAsync();

            return list.Count != 0
                ? list
                : throw new NotFoundException("No cities found for the given country!");
        }

        public async Task<City> GetCityById(int id)
        {
            return await _dbContext.Cities.FindAsync(id)
                ?? throw new NotFoundException($"City with id: {id} not found!");
        }

        public async Task<City?> GetCityByName(string name)
        {
            return await _dbContext.Cities.Where(x => x.Name == name).Include(x => x.Country).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateCity(City city)
        {
            _dbContext.Entry(city).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
