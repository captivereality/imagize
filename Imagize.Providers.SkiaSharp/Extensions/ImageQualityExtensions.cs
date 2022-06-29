using Imagize.Abstractions;
using SkiaSharp;

namespace Imagize.Providers.SkiaSharp.Extensions
{
    public static class ImageQualityExtensions
    {
        public static SKFilterQuality ToSkiaImageQuality(this ImageQuality imageQuality)
        {
            switch (imageQuality)
            {
                case ImageQuality.None:
                    return SKFilterQuality.None;
                case ImageQuality.Low: // Low quality
                    return SKFilterQuality.Low;
                case ImageQuality.Medium: // Medium quality
                    return SKFilterQuality.Medium;
                case ImageQuality.High: // High quality (default)
                default:
                    return SKFilterQuality.High;
            }
        }
    }
}
