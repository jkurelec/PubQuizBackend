using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Repository.Interface
{
    public interface IPostalCodeRepository
    {
        public Task<List<PostalCode>> GetAllPostalCodes();
        public Task<PostalCode?> GetPostalCodeById(int id);
        public Task<PostalCode?> GetPostalCodeByCode(string postalCode);
        public Task<PostalCode> AddPostalCode(PostalCode PostalCode);
        public Task<bool> UpdatePostalCode(PostalCode PostalCode);
        public Task<bool> DeletePostalCode(PostalCode PostalCode);
    }
}
