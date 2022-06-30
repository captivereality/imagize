using Imagize.Abstractions;
using Imagize.Core;
using Imagize.Core.Extensions;
using MetadataExtractor;
using Microsoft.Extensions.Options;

namespace Imagize.Services
{
    public class ImageService
    {
        private readonly ILogger<ImageService> _logger;
        private readonly ImagizeOptions _imagizeOptions;
        private readonly IHttpTools _httpTools;

        public ImageService(ILogger<ImageService> logger,
                            IOptions<ImagizeOptions> imagizeOptions,
                            IHttpTools httpTools)
        {
            _logger = logger;
            _imagizeOptions = imagizeOptions.Value;
            _httpTools = httpTools;
        }

        public async Task<bool> IsValidUri(string uri)
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

        /// <summary>
        /// Infer an Image type by looking at the first four bytes of a raw byte array
        /// Based on: https://stackoverflow.com/questions/210650/validate-image-from-file-in-c-sharp
        /// except that I already had the byte Array and didn't need the extra conversion to a stream
        /// I've checked bmp, gif, png, jpeg and tif but not tiff or jpeg2 (as I've no need for those) - MC
        /// See: https://gist.github.com/markcastle/3cc99c8e5756c7e27532900a5f8a2a93
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns>ImageFormat</returns>
        public ImageFormat GetImageFormat(byte[] byteArray)
        {

            const int INT_SIZE = 4; // We only need to check the first four bytes of the file / byte array.

            var bmp = System.Text.Encoding.ASCII.GetBytes("BM");    // BMP
            var gif = System.Text.Encoding.ASCII.GetBytes("GIF");   // GIF
            var png = new byte[] { 137, 80, 78, 71 };                       // PNG
            var tiff = new byte[] { 73, 73, 42 };                           // TIFF
            var tiff2 = new byte[] { 77, 77, 42 };                          // TIFF
            var jpeg = new byte[] { 255, 216, 255, 224 };                   // jpeg
            var jpeg2 = new byte[] { 255, 216, 255, 225 };                  // jpeg2 (canon)

            // Copy the first 4 bytes into our buffer 
            var buffer = new byte[INT_SIZE];
            Buffer.BlockCopy(byteArray, 0, buffer, 0, INT_SIZE);

            if (bmp.SequenceEqual(buffer.Take(bmp.Length)))
                return ImageFormat.BMP;

            if (gif.SequenceEqual(buffer.Take(gif.Length)))
                return ImageFormat.GIF;

            if (png.SequenceEqual(buffer.Take(png.Length)))
                return ImageFormat.PNG;

            if (tiff.SequenceEqual(buffer.Take(tiff.Length)))
                return ImageFormat.TIFF;

            if (tiff2.SequenceEqual(buffer.Take(tiff2.Length)))
                return ImageFormat.TIFF;

            if (jpeg.SequenceEqual(buffer.Take(jpeg.Length)))
                return ImageFormat.JPEG;

            if (jpeg2.SequenceEqual(buffer.Take(jpeg2.Length)))
                return ImageFormat.JPEG;

            return ImageFormat.UNKNOWN;
        }

        public async Task<IReadOnlyList<MetadataExtractor.Directory>> GetMetadata(byte[] bytes)
        {
            return await GetMetadata(new MemoryStream(bytes));
        }

        public async Task<IReadOnlyList<MetadataExtractor.Directory>> GetMetadata(Stream stream)
        {
            return await Task.Run(() => ImageMetadataReader.ReadMetadata(stream));
        }

    }
}
