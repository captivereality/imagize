using BenchmarkDotNet.Attributes;
using Imagize.Abstractions;
using Imagize.Providers.SkiaSharp;

namespace ImagizeBenchmarking
{
    [MemoryDiagnoser]
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class ImageResizer
    {
        public const string _testImageUri = "https://s3.eu-west-2.amazonaws.com/old.markcastle.com-images/s80/pic+208.jpg";

        private readonly ImageTools _imageTools;
        private readonly byte[] _originalImageBytes;

        public ImageResizer()
        {
            _imageTools = new ImageTools();
            // Download the test image once
            HttpClient httpClient = new();

            // Don't use this anywhere except here 
            var task = Task.Run(() => httpClient.GetByteArrayAsync(_testImageUri));
            task.Wait();
            _originalImageBytes = task.Result;
        }

        [Benchmark(Baseline = true)]
        public async Task<byte[]> ResizeImage()
        {
            (byte[] FileContents, int Height, int Width) result =
                await _imageTools.ResizeAsync(_originalImageBytes, 300, 0, ImageQuality.High, true);
            return result.FileContents;
        }
    }

}
