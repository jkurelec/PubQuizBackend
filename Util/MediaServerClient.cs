using PubQuizBackend.Exceptions;
using PubQuizBackend.Service.Interface;
using System.Net.Http.Headers;

namespace PubQuizBackend.Util
{
    public class MediaServerClient
    {
        private readonly HttpClient _httpClient;
        private readonly IJwtService _jwtService;
        private readonly string _basePath;

        public MediaServerClient(HttpClient httpClient, IJwtService jwtService, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _jwtService = jwtService;

            _basePath = configuration["MediaServerAddress"]
                ?? throw new ("MediaServerAddress config missing");
            _httpClient.BaseAddress = new Uri(_basePath);
        }

        private void SetAuthorizationHeader()
        {
            var token = GetAdminJwtToken();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        private string GetAdminJwtToken()
        {
            return _jwtService.GenerateAccessToken(
                userId: "0",
                username: "Backend",
                role: 3,
                teamId: null,
                app: 3
            );
        }

        public async Task<HttpResponseMessage> Post(string url, HttpContent content)
        {
            SetAuthorizationHeader();

            return await _httpClient.PostAsync(url, content);
        }

        public async Task<HttpResponseMessage> Put(string url, HttpContent content)
        {
            SetAuthorizationHeader();

            return await _httpClient.PutAsync(url, content);
        }

        public async Task<string> PostImage(string relativeUrl, IFormFile file, string fileName, string formFieldName = "image")
        {
            var content = BuildMultipartFormData(file, fileName, formFieldName);
            var response = await Post(relativeUrl, content);

            if (!response.IsSuccessStatusCode)
                throw new BadRequestException(await response.Content.ReadAsStringAsync());

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> PutImage(string relativeUrl, IFormFile file, string fileName, string formFieldName = "image")
        {
            var content = BuildMultipartFormData(file, fileName, formFieldName);
            var response = await Put(relativeUrl, content);

            if (!response.IsSuccessStatusCode)
                throw new BadRequestException(await response.Content.ReadAsStringAsync());

            return await response.Content.ReadAsStringAsync();
        }

        private static MultipartFormDataContent BuildMultipartFormData(IFormFile file, string fileName, string formFieldName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is null or empty.");

            var content = new MultipartFormDataContent();

            var streamContent = new StreamContent(file.OpenReadStream());
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");

            // The 'fileName' here sets the name the server sees for the uploaded file
            content.Add(streamContent, formFieldName, fileName);

            return content;
        }
    }
}
