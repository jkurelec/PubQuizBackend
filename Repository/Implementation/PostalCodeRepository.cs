using PubQuizBackend.Model;
using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using PubQuizBackend.Exceptions;

namespace PubQuizBackend.Repository.Implementation
{
    public class PostalCodeRepository : IPostalCodeRepository
    {
        private readonly PubQuizContext _dbContext;
        private readonly ICityRepository _cityRepository;

        public PostalCodeRepository(PubQuizContext dbContext, ICityRepository cityRepository)
        {
            _dbContext = dbContext;
            _cityRepository = cityRepository;
        }

        public async Task<PostalCode> AddPostalCode(PostalCode postalCode)
        {
            City city;

            if (await _cityRepository.GetCityByName(postalCode.City.Name) == null)
                city = await _cityRepository.AddCity(postalCode.City);
            else
                city = await _cityRepository.GetCityByName(postalCode.City.Name);

            postalCode.CityId = city.Id;

            await _dbContext.PostalCodes.AddAsync(postalCode);
            await _dbContext.SaveChangesAsync();

            return postalCode;
        }

        public Task<bool> DeletePostalCode(PostalCode postalCode)
        {
            throw new NotImplementedException();
        }

        public async Task<List<PostalCode>> GetAllPostalCodes()
        {
            return await _dbContext.PostalCodes.ToListAsync();
        }

        public async Task<PostalCode> GetPostalCodeById(int id)
        {
            return await _dbContext.PostalCodes.FindAsync(id)
                ?? throw new NotFoundException($"Postal code with id: {id} not found!");
        }

        public async Task<PostalCode?> GetPostalCodeByCode(string postalCode)
        {
            return await _dbContext.PostalCodes.Where(x => x.Code == postalCode).FirstOrDefaultAsync();
        }

        public Task<bool> UpdatePostalCode(PostalCode postalCode)
        {
            throw new NotImplementedException();
        }
    }
}
