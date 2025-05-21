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

            city = await _cityRepository.GetCityByName(postalCode.City.Name)
                ?? await _cityRepository.AddCity(postalCode.City);

            postalCode.City = city;

            await _dbContext.PostalCodes.AddAsync(postalCode);
            await _dbContext.SaveChangesAsync();

            return postalCode;
        }

  //        {
  //  "id": null,
  //  "name": "Caffe bar Gala",
  //  "address": "Ulica Viktora Cara Emina ",
  //  "postalCodeId": null,
  //  "postalCode": "51104",
  //  "cityId": null,
  //  "city": "Grad Rijeka",
  //  "country": "Hrvatska",
  //  "countryCode": "hr",
  //  "lat": 45.331433,
  //  "lon": 14.4328043
  //}

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
