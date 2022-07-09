using System.Diagnostics;

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


        public Health(long uptimeMs)
        {
            using Process proc = Process.GetCurrentProcess();

            UptimeMs = uptimeMs;
            MemoryUsageBytes = proc.PrivateMemorySize64;
            CpuCores = Environment.ProcessorCount;
        }
    }
}
