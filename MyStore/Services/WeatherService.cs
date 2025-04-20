using Microsoft.Extensions.Caching.Memory;
using MyStore.Models;
using MyStore.Services;

namespace MyStore.Services
{
    /// <summary>
    /// The WeatherService class implements the IWeatherService interface and handles retrieving weather data for a specified city. 
    /// It makes use of the OpenWeatherMap API to fetch
    /// weather data and caches the results for 10 minutes to optimize performance and reduce repeated API calls.
    /// </summary>
    public class WeatherService : IWeatherService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _config;
        private readonly string _apiKey;// Stores the API key for OpenWeatherMap API
        private const string CacheKeyPrefix = "weather_";// Prefix used for cache keys

        public WeatherService(IHttpClientFactory httpClientFactory, IMemoryCache cache, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _config = config;
            _apiKey = _config["OpenWeatherMap:ApiKey"];// Retrieves the API key from the configuration
        }

        // Asynchronously fetches weather data for the specified city, using cached data if available
        public async Task<WeatherResponse> GetWeatherAsync(string city)
        {
            // Generates the cache key using the city name (lowercase for case-insensitivity)
            var cacheKey = CacheKeyPrefix + city.ToLower();

            // Tries to get weather data from the cache
            if (_cache.TryGetValue(cacheKey, out WeatherResponse weather))
                return weather;

            var client = _httpClientFactory.CreateClient();// Creates an HTTP client to make requests
            var response = await client.GetFromJsonAsync<WeatherResponse>(
                $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric");

            // Stores the fetched weather data in the cache with a 10-minute expiration time
            _cache.Set(cacheKey, response, TimeSpan.FromMinutes(10));
            return response;
        }

        // Asynchronously refreshes the cached weather data for the specified city
        public async Task RefreshCacheAsync(string city)
        {
            // Makes an API call to fetch the latest weather data for the specified city
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetFromJsonAsync<WeatherResponse>(
                $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric");

            // Generates the cache key for the city
            var cacheKey = CacheKeyPrefix + city.ToLower();

            // Updates the cache with the new weather data
            _cache.Set(cacheKey, response, TimeSpan.FromMinutes(10));
        }
    }
}
