
using Imagize.Abstractions;
using Imagize.Providers.SkiaSharp.Extensions;
using SkiaSharp;

namespace Imagize.Providers.SkiaSharp
{
    public class ImageTools : IImageTools
    {
        public async Task<(byte[] FileContents, int Height, int Width)> ResizeAsync(
            byte[] fileContents,
            int maxWidth = 0,
            int maxHeight = 0,
            ImageQuality quality = ImageQuality.Medium,
            bool maintainAspectRatio = true,
            bool autoRotate = false)
        {
            
            if (fileContents.Length == 0)
                return (new byte[] { }, 0, 0); // nothing

            SKFilterQuality skiaQuality = quality.ToSkiaImageQuality();

            (byte[] data, int height, int width) resizeData = await Task.Run(() =>
            {
                using MemoryStream ms = new(fileContents);
                SKEncodedOrigin orientation =  SKEncodedOrigin.Default;

                if (autoRotate)
                {
                    orientation = GetOrientation(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                }

                using SKBitmap sourceBitmap = SKBitmap.Decode(ms);

                // Don't resize if not required.
                if (maxWidth == 0 && maxHeight == 0)
                {
                    return (fileContents, sourceBitmap.Height, sourceBitmap.Width);
                }

                if (maxWidth == 0 && maxHeight > 0)
                {
                    maxWidth = (int)(sourceBitmap.Width * ((float)maxHeight / sourceBitmap.Height));
                }
                else if (maxWidth > 0 && maxHeight == 0)
                {
                    maxHeight = (int)(sourceBitmap.Height * ((float)maxWidth / sourceBitmap.Width));
                }
                else if (maintainAspectRatio && maxWidth > 0 && maxHeight > 0) // Maintain aspect ratio
                {
                    if (sourceBitmap.Width > sourceBitmap.Height)
                    {
                        maxHeight = (int)(sourceBitmap.Height * ((float)maxWidth / sourceBitmap.Width));
                    }
                    else
                    {
                        maxWidth = (int)(sourceBitmap.Width * ((float)maxHeight / sourceBitmap.Height));
                    }
                }

                int width = Math.Min(maxWidth, sourceBitmap.Width);
                int height = Math.Min(maxHeight, sourceBitmap.Height);

                SKBitmap orientedBitmap = autoRotate ? AutoOrientation(sourceBitmap, orientation) : sourceBitmap;

                using SKBitmap scaledBitmap = sourceBitmap.Resize(new SKImageInfo(width, height), skiaQuality);

                using SKImage scaledImage = SKImage.FromBitmap(scaledBitmap);
                using SKData data = scaledImage.Encode();
                return (data.ToArray(), height, width);
            }
            );

            return (resizeData.data, resizeData.height, resizeData.width);
        }

        public static SKBitmap Rotate(SKBitmap bitmap, double angle)
        {
            double radians = Math.PI * angle / 180;
            float sine = (float)Math.Abs(Math.Sin(radians));
            float cosine = (float)Math.Abs(Math.Cos(radians));
            int originalWidth = bitmap.Width;
            int originalHeight = bitmap.Height;
            int rotatedWidth = (int)(cosine * originalWidth + sine * originalHeight);
            int rotatedHeight = (int)(cosine * originalHeight + sine * originalWidth);

            SKBitmap rotatedBitmap = new SKBitmap(rotatedWidth, rotatedHeight);

            using (SKCanvas surface = new SKCanvas(rotatedBitmap))
            {
                surface.Translate(rotatedWidth / 2, rotatedHeight / 2);
                surface.RotateDegrees((float)angle);
                surface.Translate(-originalWidth / 2, -originalHeight / 2);
                surface.DrawBitmap(bitmap, new SKPoint());
            }
            return rotatedBitmap;
        }

        private SKEncodedOrigin GetOrientation(Stream stream)
        {
            using SKManagedStream inputStream = new(stream);
            using SKCodec? codec = SKCodec.Create(inputStream);
            return codec.EncodedOrigin;
        }

        public static SKBitmap AutoOrientation(SKBitmap bitmap, SKEncodedOrigin orientation)
        {
            SKBitmap rotated;
            switch (orientation)
            {
                case SKEncodedOrigin.BottomRight:

                    using (var surface = new SKCanvas(bitmap))
                    {
                        surface.RotateDegrees(180, bitmap.Width / 2, bitmap.Height / 2);
                        surface.DrawBitmap(bitmap.Copy(), 0, 0);
                    }

                    return bitmap;

                case SKEncodedOrigin.RightTop:
                    rotated = new SKBitmap(bitmap.Height, bitmap.Width);

                    using (var surface = new SKCanvas(rotated))
                    {
                        surface.Translate(rotated.Width, 0);
                        surface.RotateDegrees(90);
                        surface.DrawBitmap(bitmap, 0, 0);
                    }

                    return rotated;

                case SKEncodedOrigin.LeftBottom:
                    rotated = new SKBitmap(bitmap.Height, bitmap.Width);

                    using (var surface = new SKCanvas(rotated))
                    {
                        surface.Translate(0, rotated.Height);
                        surface.RotateDegrees(270);
                        surface.DrawBitmap(bitmap, 0, 0);
                    }

                    return rotated;

                default:
                    return bitmap;
            }


        }

    }
}
