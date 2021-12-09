using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ex2_App2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet("~/app2")]
        public string Get()
        {
            return "Hi I am App2, try calling: /received-data to see what I can give back when pinged";
        }
    }
}
