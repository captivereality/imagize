using SkiaSharp;

namespace Imagize.Core
{
    public interface IImageTools
    {
      
        public Task<(byte[] FileContents, int Height, int Width)> ResizeAsync(byte[] fileContents,
            int maxWidth, int maxHeight,
            ImageQuality imageQuality = ImageQuality.Medium,
            bool maintainAspectRatio = true);

    }
}
