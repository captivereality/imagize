using Imagize.Core;
using Microsoft.AspNetCore.Mvc;

namespace Imagize.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class HealthController : Controller
    {
        /// <summary>
        /// Get the current health of this server
        /// </summary>
        /// <returns>Health Json</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(Json(new Health(Program.StartTimer.ElapsedMilliseconds)));
        }
    }
}
