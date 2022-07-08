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

        public int CpuCores { get; set; }

    }
}
