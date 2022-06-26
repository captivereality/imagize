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
    public class InfoController : ControllerBase
    {
        

        private readonly ILogger<InfoController> _logger;
        private readonly IHttpTools _httpTools;
        private readonly IImageTools _imageTools;
        private readonly ImagizeOptions _imagizeOptions;

        public InfoController(ILogger<InfoController> logger, 
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
            
            byte[] result = await _httpTools.DownloadAsync(uri);

            return result.Count().ToString();
        }

        private async Task<bool> IsValidUri(string uri)
        {

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
            
            _logger.LogInformation("Allowed Origins:{origins}", _imagizeOptions.AllowedOrigins);

            return true;
        }
    }
}