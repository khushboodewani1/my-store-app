using Microsoft.AspNetCore.Mvc;
using MyStore.Services;

namespace MyStore.Controllers
{
    /// <summary>
    /// The WeatherController handles displaying weather data for a specified city. 
    /// It interacts with the IWeatherService to fetch weather information and display it on the view. 
    /// </summary>
    public class WeatherController : Controller
    {
        private readonly IWeatherService _weatherService;

        // Constructor to inject the weather service dependency
        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Checking if a city is available in TempData (for persistence across redirects)
            if (TempData["city"] is string city && !string.IsNullOrWhiteSpace(city))
            {
                var data = await _weatherService.GetWeatherAsync(city);

                // Storing the city name in the ViewBag to display on the view
                ViewBag.City = city;
                return View(data);
            }

            // Initial view with no weather data
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string city)
        {
            try
            {
                // Try fetching weather for the entered city
                var data = await _weatherService.GetWeatherAsync(city);
                ViewBag.City = city;
                return View(data);
            }
            catch (HttpRequestException)
            {
                // If the API returned 404 or other non-success
                ModelState.AddModelError(string.Empty,
                    $"Could not find weather data for '{city}'. Please check the city name and try again.");
                return View();
            }
        }


        // POST: Refresh weather data for the previously entered city
        [HttpPost]
        public IActionResult Refresh(string city)
        {
            TempData["city"] = city; // Store city temporarily
            return RedirectToAction("Index");
        }

    }
}
