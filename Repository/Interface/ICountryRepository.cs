using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Repository.Interface
{

    public interface ICountryRepository
    {
        Task<List<Country>> GetAllCountries();
        Task<Country> GetCountryById(int id);
        Task<Country?> GetCountryByName(string name);
        Task<Country> AddCountry(Country country);
        Task<bool> UpdateCountry(Country country);
        Task<bool> DeleteCountry(Country country);
    }
}
