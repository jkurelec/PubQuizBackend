using Microsoft.EntityFrameworkCore;
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
            Country country;

            if (await _countryRepository.GetCountryByName(city.Country.Name) == null)
                country = await _countryRepository.AddCountry(city.Country);
            else
                country = await _countryRepository.GetCountryByName(city.Country.Name);

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
            return await _dbContext.Cities.Where(x => x.CountryId == id).ToListAsync();
        }

        public async Task<City?> GetCityById(int id)
        {
            return await _dbContext.Cities.FindAsync(id);
        }

        public async Task<City?> GetCityByName(string name)
        {
            return await _dbContext.Cities
                .Where(x => x.Name == name)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateCity(City city)
        {
            _dbContext.Entry(city).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return false;
        }
    }
}
