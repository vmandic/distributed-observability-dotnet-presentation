using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ex2_App1.Controllers
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

        [HttpGet("~/app1")]
        public string Get()
        {
            return "Hi I am App1, to see tracing try calling: https://localhost:60001/get-data";
        }
    }
}
