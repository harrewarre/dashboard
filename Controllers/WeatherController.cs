using System;
using System.Net.Http;
using System.Threading.Tasks;
using dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace dashboard.Controllers
{

    [Route("api/weather/v1")]
    public class WeatherController : Controller
    {
        private readonly string WeatherApiKey;

        public WeatherController(IOptions<ApiKeys> options)
        {
            WeatherApiKey = options.Value.WeatherApiKey;
        }

        [HttpGet]
        public async Task<ActionResult> GetWeatherInfo()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var json = await client.GetStringAsync(WeatherApiKey);
                    dynamic result = JsonConvert.DeserializeObject(json);

                    var weatherResponse = new
                    {
                        description = result.weather[0]?.description ?? string.Empty,
                        city = result.name,
                        high = result.main.temp_max,
                        low = result.main.temp_min,
                        humidity = result.main.humidity
                    };

                    return Ok(weatherResponse);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to load weather data: { ex.Message }");
            }
        }
    }
}