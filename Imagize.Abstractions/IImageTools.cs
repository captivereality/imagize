
namespace Imagize.Abstractions
{
    public interface IImageTools
    {
      
        public Task<(byte[] FileContents, int Height, int Width)> ResizeAsync(byte[] imageBytes,
            int maxWidth, int maxHeight,
            ImageQuality imageQuality = ImageQuality.Medium,
            bool maintainAspectRatio = true,
            bool autoRotate = false);
        public Task<(byte[] FileContents, int Height, int Width)> CropAsync(byte[] imageBytes,
            int left,
            int top,
            int right,
            int bottom);

        public Task<(byte[] FileContents, int Height, int Width)> AddTextAsync(byte[] imageBytes, 
            string text, 
            int x,
            int y, 
            int textSize = 20,
            CanvasOrigin canvasOrigin = CanvasOrigin.TopLeft);
    }
}
