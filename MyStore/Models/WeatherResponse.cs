using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStore.Models
{
    public class WeatherResponse
    {
        public MainWeatherData Main { get; set; }
        public List<WeatherDescription> Weather { get; set; }
        public string Name { get; set; }
    }

    public class MainWeatherData
    {
        public float Temp { get; set; }
        public float Feels_like { get; set; }
        public int Humidity { get; set; }
    }

    public class WeatherDescription
    {
        public string Description { get; set; }
    }
}
