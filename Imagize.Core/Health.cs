using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Imagize.Core
{
    public class Health
    {
        /// <summary>
        /// Uptime in Milliseconds
        /// </summary>
        public long UptimeMs { get; set; }

        /// <summary>
        /// See: https://devblogs.microsoft.com/dotnet/using-net-and-docker-together-dockercon-2019-update/
        /// See: https://github.com/dotnet/runtime/issues/28990
        /// </summary>
        public long MemoryUsageBytes { get; set; }

        /// <summary>
        /// Number of CPU Cores Available
        /// </summary>
        public int CpuCores { get; set; }

        public string Platform { get; set; }
        
        public string Os { get; set; }


        public Health(long uptimeMs)
        {
            using Process proc = Process.GetCurrentProcess();

            UptimeMs = uptimeMs;
            MemoryUsageBytes = proc.PrivateMemorySize64;
            CpuCores = Environment.ProcessorCount;
            Platform = GetOperatingSystem().ToString();
            Os = Environment.OSVersion.ToString();
        }

        public static OSPlatform GetOperatingSystem()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OSPlatform.OSX;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OSPlatform.Linux;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OSPlatform.Windows;
            }

            throw new Exception("Cannot determine operating system!");
        }
    }
}
