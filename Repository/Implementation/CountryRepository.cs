using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Repository.Interface;
using System.Data;

namespace PubQuizBackend.Repository.Implementation
{
    public class CountryRepository : ICountryRepository
    {
        private readonly PubQuizContext _dbContext;

        public CountryRepository(PubQuizContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Country> AddCountry(Country country)
        {
            await _dbContext.AddAsync(country);
            await _dbContext.SaveChangesAsync();

            return country;
        }

        public async Task<bool> DeleteCountry(Country country)
        {
            _dbContext.Remove(country);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<Country>> GetAllCountries()
        {
            return await _dbContext.Countries.ToListAsync();
        }

        public async Task<Country?> GetCountryById(int id)
        {
            return await _dbContext.Countries.FindAsync(id);
        }

        public async Task<Country?> GetCountryByName(string name)
        {
            return await _dbContext.Countries.Where(x => x.Name == name).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateCountry(Country country)
        {
            _dbContext.Entry(country).State = EntityState.Modified;

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
