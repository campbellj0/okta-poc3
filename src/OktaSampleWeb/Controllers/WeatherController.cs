using Microsoft.AspNetCore.Mvc;
using OktaSampleWeb.Models;
using OktaSampleWeb.Services.Weather;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OktaSampleWeb.Controllers
{
    public class WeatherController : Controller
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        public async Task<ActionResult> Index()
        {
            var model = await _weatherService.GetWeatherAsync();
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}