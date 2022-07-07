using Imagize.Abstractions;
using Imagize.Core;
using Imagize.Core.Extensions;
using Imagize.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
        private readonly ImageService _imageService;

        public ImageController(ILogger<ImageController> logger,
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
        /// Resize an Image with optional auto rotation
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="imageQuality"></param>
        /// <param name="maintainAspectRatio"></param>
        /// <param name="autoRotate"></param>
        /// <returns>string</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public async Task<IActionResult> Resize(
            [FromQuery] string uri,
            [FromQuery] int width = 0,
            [FromQuery] int height = 0,
            [FromQuery] ImageQuality imageQuality = ImageQuality.Medium,
            [FromQuery] bool maintainAspectRatio = true,
            [FromQuery] bool autoRotate = true)
        {

            if (!await _imageService.IsValidUri(uri))
                return NotFound("Invalid Uri or file type");

            _logger.LogInformation("Resize");

            byte[] imageBytes = await _httpTools.DownloadAsync(uri);

            if (imageBytes.Length == 0)
                return NotFound("Invalid Image");

            // Get the file format.
            // Note for more formats we could use..
            // https://github.com/drewnoakes/metadata-extractor/wiki/File-Type-Detection
            ImageFormat imageType = _imageService.GetImageFormat(imageBytes);
            _logger.LogInformation("Image type detected:{imageType}", imageType.ToString());

            (byte[] FileContents, int Height, int Width) result =
                await _imageTools.ResizeAsync(imageBytes, width, height, imageQuality, maintainAspectRatio, autoRotate);

            return File(result.FileContents, $"image/{imageType.ToString().ToLower()}");

        }

    }
}