﻿using Imagize.Core;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Imagize.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class HealthController : Controller
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            Process proc = Process.GetCurrentProcess();

            Health health = new()
            {
                UptimeMs = Program.StartTimer.ElapsedMilliseconds,
                MemoryUsageBytes = proc.PrivateMemorySize64
            };

            return Ok(Json(health));
        }
    }
}