using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ex1_HelloWorldLogging.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var name = "ATD-16";

            // log info level
            _logger.LogInformation("Hello logger!");

            // log warn level
            _logger.LogWarning("Hello logger warning!");

            // log err level
            _logger.LogError("Hello logger error!");

            // log ftl level
            _logger.LogCritical("Hello logger FATAL / CRITICAL!!!");

            // log err level + excp
            try
            {
                throw new Exception("Whoopsie daisy! Something went wrong...");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hello logger error!");
            }

            // DO NOT DO THIS: log string interpolated
            _logger.LogInformation($"Hello {name}! (string interpolated)", name);

            // BUT DO THIS: log interpolated values to save parts as structured logging properties
            _logger.LogInformation("Hello {author}! (log property interpolated)", name);

            // use event ID to find quicker in windows event log, make sure you export to windows EventLog with EventLog log provider registration - windows only!
            _logger.LogInformation(123, "Hello {author}!", name);

            // create use custom event IDs with title
            var e = new EventId(5555, "Some event");
            _logger.LogWarning(e, "Whoops some warning occured at (UTC+0): {timestamp}", DateTime.UtcNow);

            // enrich the log with contextual data
            using var logscope = _logger.BeginScope("this is a scope with user '{user}' and role '{role}'", "user 123", "finance");
            _logger.LogInformation("This is prop enriched message from log context");

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
