using MyStore.Models;

namespace MyStore.Services
{
    /// <summary>
    /// The IWeatherService interface defines two asynchronous methods for interacting with weather data:
    /// </summary>
    public interface IWeatherService
    {
        Task<WeatherResponse> GetWeatherAsync(string city);
        Task RefreshCacheAsync(string city);
    }
}
