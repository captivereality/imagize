using Imagize.Abstractions;
using Imagize.Core;
using Imagize.Core.Extensions;
using Imagize.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Imagize.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class InfoController : ControllerBase
    {
        

        private readonly ILogger<InfoController> _logger;
        private readonly IHttpTools _httpTools;
        private readonly IImageTools _imageTools;
        private readonly ImagizeOptions _imagizeOptions;
        private readonly ImageService _imageService;

        public InfoController(ILogger<InfoController> logger, 
            IHttpTools httpTools,
            IImageTools imageTools,
            IOptions<ImagizeOptions> imagizeOptions,
            ImageService imageService)
        {
            _logger = logger;
            _httpTools = httpTools;
            _imageTools = imageTools;
            _imagizeOptions = imagizeOptions.Value;
            _imageService = imageService;
        }

       
        /// <summary>
        /// Get Exif Data
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>string</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string uri)
        {
            
            if (!await _imageService.IsValidUri(uri))
                return NotFound("Invalid Uri or FileType");

            _logger.LogInformation("Get");            

            byte[] result = await _httpTools.DownloadAsync(uri);

            var metaData = await _imageService.GetMetadata(result);

            return Ok(JsonSerializer.Serialize(metaData));

            // return result.Count().ToString();
        }


    }
}