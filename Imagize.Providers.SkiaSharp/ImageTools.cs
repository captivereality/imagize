
using HarfBuzzSharp;
using Imagize.Abstractions;
using Imagize.Providers.SkiaSharp.Extensions;
using Microsoft.Extensions.Options;
using SkiaSharp;
using Topten.RichTextKit;

namespace Imagize.Providers.SkiaSharp
{
    public class ImageTools : IImageTools
    {
        private readonly ImagizeOptions _imagizeOptions;
        public ImageTools(IOptions<ImagizeOptions> imagizeOptions)
        {
            _imagizeOptions = imagizeOptions.Value;
        }

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
                    SKEncodedOrigin orientation = SKEncodedOrigin.Default;

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

        public async Task<(byte[] FileContents, int Height, int Width)>
            CropAsync(byte[] imageBytes,
                int left,
                int top,
                int right,
                int bottom)
        {
            // write code to crop an image

            if (imageBytes.Length == 0)
                return (new byte[] { }, 0, 0); // nothing


            (byte[] data, int height, int width) cropData = await Task.Run(() =>
                {

                    using MemoryStream ms = new(imageBytes);

                    using SKBitmap sourceBitmap = SKBitmap.Decode(ms);

                    int sourceWidth = sourceBitmap.Width;
                    int sourceHeight = sourceBitmap.Height;

                    // We can't crop if the image isn't as big as the top and left
                    if (left > sourceWidth || top > sourceHeight)
                    {
                        return (imageBytes, sourceHeight, sourceWidth);
                    }

                    // Restrict right to the actual source width
                    // If right not supplied clamp to source width
                    if (right == 0 || right > sourceWidth)
                        right = sourceWidth;

                    // restrict bottom to the actual source height
                    // if bottom not supplied, clamp to source height
                    if (bottom == 0 || bottom > sourceHeight)
                        bottom = sourceHeight;

                    // Setup a bounding box for our new rectangle
                    SKRectI newRect = SKRectI.Create(left, top, right, bottom);

                    using SKImage? image = SKImage.FromBitmap(sourceBitmap);
                    SKBitmap? newBitmap;
                    try
                    {
                        using SKImage? subset = image.Subset(newRect);
                        newBitmap = SKBitmap.FromImage(subset);

                    }
                    catch (Exception ex)
                    {
                        //todo: Structured logging instead
                        Console.WriteLine($"Exception:{ex}") ;
                        return (imageBytes, sourceHeight, sourceWidth);
                    }
                    
                    using SKImage croppedImage = SKImage.FromBitmap(newBitmap);
                    using SKData data = croppedImage.Encode();

                    int width = croppedImage.Width;
                    int height = croppedImage.Height;

                    return (data.ToArray(), height, width);
                }
            );
            return cropData;
        }

        //public async Task<(byte[] image, int Height, int Width)> AddTextAsync(byte[] imageBytes, 
        //    string text, 
        //    int x, 
        //    int y, 
        //    int textSize = 20)
        //{

        //    SKBitmap sourceBitmap = await GetBitmap(imageBytes);

        //    SKBitmap bitmapWithText = await AddTextAsync(sourceBitmap, text, x, y, textSize);

        //    using SKImage croppedImage = SKImage.FromBitmap(bitmapWithText);
        //    using SKData data = croppedImage.Encode();
        //    return (data.ToArray(), croppedImage.Width, croppedImage.Height);

        //}

        private async Task<SKBitmap> GetBitmap(byte[] imageBytes)
        {
            using MemoryStream ms = new(imageBytes);
            using SKBitmap sourceBitmap = SKBitmap.Decode(ms);
            return sourceBitmap;
            
            //SKBitmap? bitmap = await Task.Run(() =>
            //{
            //    using MemoryStream ms = new(imageBytes);
            //    using SKBitmap sourceBitmap = SKBitmap.Decode(ms);
            //    return sourceBitmap;
            //});

            //return bitmap;
        }
        
        #region ------------------------- Internal Methods -------------------------


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
        //public async Task<SKBitmap> AddTextAsync(byte[] imageBytes, string text, int x, int y, int textSize = 20)
        //{

         

        //    //SKBitmap sourceBitmap = await Task.Run(() =>
        //    //{
        //    //    using SKCanvas canvas = new SKCanvas(bitmap);
        //    //    using SKPaint paint = new SKPaint();
        //    //    paint.Color = SKColors.White;
        //    //    paint.TextSize = textSize;
        //    //    canvas.DrawText(text, x, y, paint);
        //    //    return bitmap;
        //    //});

        //    //return sourceBitmap;
        //}

        public async Task<(byte[] FileContents, int Height, int Width)> AddTextAsync(byte[] imageBytes, 
            string text, 
            int x, 
            int y, 
            int textSize, 
            CanvasOrigin canvasOrigin = CanvasOrigin.TopLeft,
            byte red = 255,
            byte green = 255,
            byte blue = 255,
            byte alpha = 128)
        {
            // Todo: Add some validation to make sure max text length isn't going to kill us

            using MemoryStream ms = new(imageBytes);
            using SKBitmap sourceBitmap = SKBitmap.Decode(ms);

            using SKCanvas canvas = new(sourceBitmap);

            // Little hack so that we can draw from the bottom also
            if (canvasOrigin == CanvasOrigin.BottomLeft)
                y = sourceBitmap.Height - y;

            // This is currently unsupported on Linux
            if (_imagizeOptions.TextSupport == TextSupport.RichTextKit)
            {
                RichString? rs = new RichString()
                    .TextColor(new SKColor(red, green, blue, alpha))
                    .Alignment(TextAlignment.Center)
                    .FontFamily("Segoe UI")
                    .MarginBottom(20)
                    .Add(text, fontSize: textSize, fontWeight: 300, fontItalic: false);

                rs.Paint(canvas, new SKPoint(x, y));
            }
            else
            {
                using SKPaint paint = new SKPaint();
                paint.Color = new SKColor(red, green, blue, alpha); // SKColors.White;
                paint.TextSize = textSize;
                paint.Typeface = SKTypeface.Default;
                canvas.DrawText(text, x, y, paint);
                //// return sourceBitmap;
            }


            using SKImage croppedImage = SKImage.FromBitmap(sourceBitmap);
            using SKData data = croppedImage.Encode();
            return (data.ToArray(), croppedImage.Width, croppedImage.Height);
        }

        #endregion ------------------------- Internal Methods -------------------------
    }
}
