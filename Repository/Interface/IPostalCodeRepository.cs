using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Repository.Interface
{
    public interface IPostalCodeRepository
    {
        Task<List<PostalCode>> GetAllPostalCodes();
        Task<PostalCode> GetPostalCodeById(int id);
        Task<PostalCode?> GetPostalCodeByCode(string postalCode);
        Task<PostalCode> AddPostalCode(PostalCode PostalCode);
        Task<bool> UpdatePostalCode(PostalCode PostalCode);
        Task<bool> DeletePostalCode(PostalCode PostalCode);
    }
}
