using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Repository.Interface
{

    public interface ICountryRepository
    {
        public Task<List<Country>> GetAllCountries();
        public Task<Country> GetCountryById(int id);
        public Task<Country?> GetCountryByName(string name);
        public Task<Country> AddCountry(Country country);
        public Task<bool> UpdateCountry(Country country);
        public Task<bool> DeleteCountry(Country country);
    }
}
