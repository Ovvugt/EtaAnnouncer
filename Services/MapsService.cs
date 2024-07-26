using static ETA_announcer.Controllers.TravelTimeController;
using System.Text.Json;
using System.Text;
using ETA_announcer.Models;

namespace ETA_announcer.Services
{
    public interface IMapsService 
    {
        Task<GoogleComputeRoutesResponse?> GetRouteAsync(GoogleComputeRoutesRequest request, string apiKey);
    }
    public class MapsService : IMapsService
    {
        private readonly HttpClient _httpClient;
        private const string _googleUri = "https://routes.googleapis.com/";
        public MapsService()
        {
            _httpClient = new() { BaseAddress = new Uri(_googleUri) };
        }
        public async Task<GoogleComputeRoutesResponse?> GetRouteAsync(GoogleComputeRoutesRequest request, string apiKey)
        {

            var requestUri = $"directions/v2:computeRoutes";

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(JsonSerializer.Serialize(request, options), Encoding.UTF8, "application/json")
            };
            httpRequestMessage.Headers.Add("X-Goog-Api-Key", apiKey);
            httpRequestMessage.Headers.Add("X-Goog-FieldMask", "routes.duration");
            var response = await _httpClient.SendAsync(httpRequestMessage);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var responseString = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<GoogleComputeRoutesResponse>(responseString, options);
        }
    }
}
