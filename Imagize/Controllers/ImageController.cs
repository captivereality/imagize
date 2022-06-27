using Imagize.Core;
using Imagize.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections;
using static System.Net.Mime.MediaTypeNames;

namespace Imagize.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ImageController : ControllerBase
    {
        

        private readonly ILogger<ImageController> _logger;
        private readonly IHttpTools _httpTools;
        private readonly IImageTools _imageTools;
        private readonly ImagizeOptions _imagizeOptions;

        public ImageController(ILogger<ImageController> logger, 
            IHttpTools httpTools,
            IImageTools imageTools,
            IOptions<ImagizeOptions> imagizeOptions)
        {
            _logger = logger;
            _httpTools = httpTools;
            _imageTools = imageTools;
            _imagizeOptions = imagizeOptions.Value;
        }

        /// <summary>
        /// Resize Image
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="imageQuality"></param>
        /// <param name="maintainAspectRatio"></param>
        /// <returns>string</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public async Task<IActionResult> Resize(
            [FromQuery] string uri, 
            [FromQuery] int width = 0, 
            [FromQuery] int height = 0, 
            [FromQuery] ImageQuality imageQuality = ImageQuality.Medium,
            [FromQuery] bool maintainAspectRatio = true)
        {

            if (!await IsValidUri(uri))
                return NotFound("Invalid Uri or file type");

            _logger.LogInformation("Resize");

            byte[] imageBytes = await _httpTools.DownloadAsync(uri);

            (byte[] FileContents, int Height, int Width) result = 
                await _imageTools.ResizeAsync(imageBytes, width, height, imageQuality, maintainAspectRatio);

            return File(result.FileContents, "image/jpeg");

        }

        /// <summary>
        /// Get Exif Data
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>string</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public async Task<string> Get([FromQuery] string uri)
        {

            if (!await IsValidUri(uri))
                return "Invalid Uri or FileType";

            _logger.LogInformation("Get");

            byte[] result = await _httpTools.DownloadAsync(uri);

            return result.Count().ToString();
        }

        private async Task<bool> IsValidUri(string uri)
        {
            _logger.LogInformation("AllowedOrigins:{AllowedOrigins}", _imagizeOptions.AllowedOrigins);
            _logger.LogInformation("AllowedFileTypes:{AllowedFileTypes}", _imagizeOptions.AllowedFileTypes);

            if (string.IsNullOrEmpty(uri))
                return false;
            
            if (_imagizeOptions.AllowedOrigins is not null)
            {
                if (!_httpTools.IsValidOrigin(uri, _imagizeOptions.AllowedOrigins))
                    return false;
            }

            if (_imagizeOptions.AllowedFileTypes is not null)
            {
                if (!_httpTools.IsValidFileType(uri, _imagizeOptions.AllowedFileTypes))
                    return false;
            }
          
            return true;
        }
    }
}