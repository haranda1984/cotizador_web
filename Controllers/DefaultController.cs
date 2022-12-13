using Microsoft.AspNetCore.Mvc;

namespace HeiLiving.Quotes.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class DefaultController : ControllerBase
    {        
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }
    }
}