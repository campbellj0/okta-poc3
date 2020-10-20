using OktaSampleWeb.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OktaSampleWeb.Services.Weather
{
    public interface IWeatherService
    {
        Task<IEnumerable<WeatherForecast>> GetWeatherAsync();
    }
}
