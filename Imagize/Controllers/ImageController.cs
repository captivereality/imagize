using Imagize.Abstractions;
using Imagize.Core;
using Imagize.Core.Extensions;
using Imagize.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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

        /// <summary>
        /// Crop an image
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public async Task<IActionResult> Crop(
            [FromQuery] string uri,
            [FromQuery] int left = 0,
            [FromQuery] int top = 0,
            [FromQuery] int right = 0,
            [FromQuery] int bottom = 0
            )
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
                await _imageTools.CropAsync(imageBytes, left, top, right, bottom);

            return File(result.FileContents, $"image/{imageType.ToString().ToLower()}");

        }

        /// <summary>
        ///  Watermark an image with Text
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="text"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="textSize"></param>
        /// <param name="canvasOrigin"></param>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public async Task<IActionResult> WatermarkText(
            [FromQuery] string uri,
            [FromQuery] string text,
            [FromQuery] int x = 0,
            [FromQuery] int y = 0,
            [FromQuery] int textSize = 0,
            [FromQuery] CanvasOrigin canvasOrigin = CanvasOrigin.TopLeft,
            [FromQuery] int red = 255,
            [FromQuery] int green = 255,
            [FromQuery] int blue = 255,
            [FromQuery] int alpha = 128
        )
        {

            if (!await _imageService.IsValidUri(uri))
                return NotFound("Invalid Uri or file type");

            _logger.LogInformation("Resize");

            byte[] imageBytes = await _httpTools.DownloadAsync(uri);
            byte r = (byte)Math.Clamp(red, 0, 255);
            byte g = (byte)Math.Clamp(green, 0, 255);
            byte b = (byte)Math.Clamp(blue, 0, 255);
            byte a = (byte)Math.Clamp(alpha, 0, 255);

            if (imageBytes.Length == 0)
                return NotFound("Invalid Image");

            // Get the file format.
            // Note for more formats we could use..
            // https://github.com/drewnoakes/metadata-extractor/wiki/File-Type-Detection
            ImageFormat imageType = _imageService.GetImageFormat(imageBytes);
            _logger.LogInformation("Image type detected:{imageType}", imageType.ToString());

            (byte[] FileContents, int Height, int Width) result =
                await _imageTools.AddTextAsync(imageBytes, text, x, y, textSize, canvasOrigin, r, g, b, a);

            return File(result.FileContents, $"image/{imageType.ToString().ToLower()}");

        }

    }
}