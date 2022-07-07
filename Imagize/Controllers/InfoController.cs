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
        /// Get All Exif Data for a source image
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>string</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string uri)
        {

            // Guard against invalid uri
            if (!await _imageService.IsValidUri(uri))
                return NotFound("Invalid Uri or FileType");

            // Download the source image
            byte[] result = await _httpTools.DownloadAsync(uri);

            // Ensure we've got some bytes at least
            if (result.Length <= 0)
                return NotFound("Invalid File");

            // Extract metadata from the image
            return Ok(JsonSerializer.Serialize(await _imageService.GetMetadata(result)));

        }


    }
}