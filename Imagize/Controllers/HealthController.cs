using Imagize.Core;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Imagize.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class HealthController : Controller
    {
        /// <summary>
        /// Get the current health of this server
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            Process proc = Process.GetCurrentProcess();

            Health health = new()
            {
                UptimeMs = Program.StartTimer.ElapsedMilliseconds,
                MemoryUsageBytes = proc.PrivateMemorySize64,
                CpuCores = Environment.ProcessorCount
            };

            return Ok(Json(health));
        }
    }
}
