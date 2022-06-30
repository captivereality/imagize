using BenchmarkDotNet.Running;

namespace ImagizeBenchmarking
{
    internal class Program
    {
        
        static async Task Main(string[] args)
        {

            var summary = BenchmarkRunner.Run(typeof(ImageResizer).Assembly);
        }
    }
}