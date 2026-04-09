using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;

namespace GrowsmartAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        [HttpGet("current")]
        public IActionResult GetCurrentWeather()
        {
            var now = DateTime.Now;
            var temp = now.Hour >= 11 && now.Hour <= 17 ? 34 : 28;
            var condition = now.Hour >= 6 && now.Hour <= 18 ? "Sunny" : "Clear Sky";
            var icon = now.Hour >= 6 && now.Hour <= 18 ? "sunny" : "clear";
            
            return Ok(new
            {
                temperature = temp,
                condition = condition,
                icon = icon,
                location = "Your Garden",
                humidity = 65,
                windSpeed = 12,
                feelsLike = temp + 2,
                uvIndex = "High",
                message = temp > 30 ? "High Heat! Water your plants early." : "Ideal growing conditions today."
            });
        }

        [HttpGet("hourly")]
        public IActionResult GetHourlyForecast()
        {
            var now = DateTime.Now;
            var hourly = new List<object>();
            
            for (int i = 0; i < 8; i++)
            {
                var time = now.AddHours(i);
                var temp = 28 + (i % 5);
                hourly.Add(new { 
                    time = time.ToString("HH:00"), 
                    temperature = temp, 
                    rainProbability = i * 5 % 20, 
                    icon = temp > 30 ? "sunny" : "partly_cloudy" 
                });
            }
            
            return Ok(hourly);
        }

        [HttpGet("impact")]
        public IActionResult GetWeatherImpact()
        {
            return Ok(new List<object> { 
                new { 
                    id = 1, 
                    title = "High Transpiration Risk", 
                    description = "Heat above 30°C increases moisture loss. Water deeply in early morning.", 
                    riskLevel = "High" 
                },
                new { 
                    id = 2, 
                    title = "Optimal UV Synthesis", 
                    description = "Strong sunlight is ideal for fruiting crops like tomatoes and peppers.", 
                    riskLevel = "Medium" 
                },
                new { 
                    id = 3, 
                    title = "Humidity Management", 
                    description = "Humidity at 65% is stable. No immediate risk of fungal growth.", 
                    riskLevel = "Low" 
                }
            });
        }

        [HttpGet("alerts")]
        public IActionResult GetWeatherAlerts()
        {
            var now = DateTime.Now;
            var alerts = new List<object>();
            
            if (now.Hour < 15) {
                alerts.Add(new { 
                    type = "Heat Warning", 
                    message = "Peak temperatures expected at 3:00 PM. Hydrate your garden.", 
                    severity = "High" 
                });
            }

            return Ok(alerts);
        }

        [HttpGet("forecast")]
        public IActionResult Get7DayForecast()
        {
            var now = DateTime.Now;
            var forecast = new List<object>();
            string[] days = { "Tomorrow", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday", "Monday" };
            string[] conditions = { "Sunny", "Cloudy", "Clear", "Partly Cloudy", "Sunny", "Clear", "Cloudy" };
            string[] icons = { "sunny", "cloudy", "sunny", "cloudy", "sunny", "sunny", "cloudy" };
            string[] advice = { 
                "Normal watering", 
                "Lower evaporation - water less", 
                "Great day for pruning", 
                "Check for pests", 
                "Ideal for fertilizing", 
                "Maintain soil moisture", 
                "Good transplanting weather" 
            };

            for (int i = 0; i < 7; i++)
            {
                forecast.Add(new { 
                    day = days[i], 
                    temperature = 28 + (i % 4), 
                    condition = conditions[i], 
                    icon = icons[i], 
                    advice = advice[i] 
                });
            }
            
            return Ok(forecast);
        }

        [HttpGet]
        public IActionResult GetWeather()
        {
            return GetCurrentWeather();
        }
    }
}
