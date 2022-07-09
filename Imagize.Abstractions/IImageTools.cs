
namespace Imagize.Abstractions
{
    public interface IImageTools
    {
      
        public Task<(byte[] FileContents, int Height, int Width)> ResizeAsync(byte[] fileContents,
            int maxWidth, int maxHeight,
            ImageQuality imageQuality = ImageQuality.Medium,
            bool maintainAspectRatio = true,
            bool autoRotate = false);
        Task<(byte[] FileContents, int Height, int Width)> CropAsync(byte[] imageBytes,
            int left,
            int top,
            int right,
            int bottom);
    }
}
