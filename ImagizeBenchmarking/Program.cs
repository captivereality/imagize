using System;
using System.Net.Http;
using BenchmarkDotNet.Running;
using Imagize.Core;

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