using PubQuizBackend.Model.Dto.QuizAnswerDto;
using PubQuizBackend.Service.Interface;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PubQuizBackend.Util
{
    public class PythonServerClient
    {
        private readonly HttpClient _httpClient;
        private readonly IJwtService _jwtService;
        private readonly string _basePath;

        public PythonServerClient(HttpClient httpClient, IJwtService jwtService, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _jwtService = jwtService;

            _basePath = configuration["PythonServerAddress"]
                ?? throw new("PythonServerAddress config missing");
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

        public async Task<ProtectedResponse?> CreateRecommendations(int editionId)
        {
            SetAuthorizationHeader();

            var response = await _httpClient.PostAsJsonAsync("/recommendations", new { id = editionId });
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProtectedResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<string?> ExtractSentencesFromImage(IFormFile image)
        {
            SetAuthorizationHeader();

            using var content = new MultipartFormDataContent();

            using var stream = image.OpenReadStream();
            var imageContent = new StreamContent(stream);
            var extension = Path.GetExtension(image.FileName).ToLower();
            var contentType = extension switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".bmp" => "image/bmp",
                _ => "image/jpeg" // fallback
            };
            imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

            content.Add(imageContent, "image", image.FileName);

            var response = await _httpClient.PostAsync("/sentence", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            return json;
        }
    }

    public class ProtectedResponse
    {
        public string Message { get; set; } = string.Empty;
    }
}
