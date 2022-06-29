//using Imagize.Abstractions;
//using Imagize.Core.Extensions;
//using SkiaSharp;

//namespace Imagize.Core
//{
//    public class ImageTools : IImageTools
//    {
      
//        public async Task<(byte[] FileContents, int Height, int Width)> ResizeAsync(
//            byte[] fileContents, 
//            int maxWidth = 0, 
//            int maxHeight = 0, 
//            ImageQuality quality = ImageQuality.Medium,
//            bool maintainAspectRatio = true)
//        {
//            if (fileContents.Length == 0)
//                return (new byte[] { }, 0, 0); // nothing

//            SKFilterQuality skiaQuality = quality.ToSkiaImageQuality();

//            (byte[] data, int height, int width) resizeData = await Task.Run(() =>
//                {
//                    using MemoryStream ms = new(fileContents);
//                    using SKBitmap sourceBitmap = SKBitmap.Decode(ms);

                  

//                    // Don't resize if not required.
//                    if (maxWidth == 0 && maxHeight == 0)
//                    {
//                        return (fileContents, sourceBitmap.Height, sourceBitmap.Width);
//                    }

//                    if (maxWidth == 0 && maxHeight > 0)
//                    {
//                        maxWidth = (int)(sourceBitmap.Width * ((float)maxHeight / sourceBitmap.Height));
//                    }
//                    else if (maxWidth > 0 && maxHeight == 0)
//                    {
//                        maxHeight = (int)(sourceBitmap.Height * ((float)maxWidth / sourceBitmap.Width));
//                    }
//                    else if (maintainAspectRatio && maxWidth > 0 && maxHeight > 0) // Maintain aspect ratio
//                    {
//                        if (sourceBitmap.Width > sourceBitmap.Height)
//                        {
//                            maxHeight = (int)(sourceBitmap.Height * ((float)maxWidth / sourceBitmap.Width));
//                        }
//                        else
//                        {
//                            maxWidth = (int)(sourceBitmap.Width * ((float)maxHeight / sourceBitmap.Height));
//                        }
//                    }

//                    int width = Math.Min(maxWidth, sourceBitmap.Width);
//                    int height = Math.Min(maxHeight, sourceBitmap.Height);

//                    using SKBitmap scaledBitmap = sourceBitmap.Resize(new SKImageInfo(width, height), skiaQuality);
//                    using SKImage scaledImage = SKImage.FromBitmap(scaledBitmap);
//                    using SKData data = scaledImage.Encode();
//                    return (data.ToArray(), height, width);
//                }
//            );

//            return (resizeData.data, resizeData.height, resizeData.width);
//        }

     

//    }
//}
