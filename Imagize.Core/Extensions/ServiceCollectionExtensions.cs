using Imagize.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Imagize.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddImagize(this IServiceCollection services,
            Action<ImagizeOptions>? imagizeOptionsAction = null)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            // Setup Options...
            if (imagizeOptionsAction is not null)
            {
                // Build a new configuration into which we'll merge the config supplied
                ImagizeOptions? imagizeOptions = new();

                // Merge the configuration (if one is supplied)
                imagizeOptionsAction?.Invoke(imagizeOptions);

                services.AddOptions<ImagizeOptions>()
                    .Configure(imagizeOptionsAction);
            }
            else
            {
                services.AddOptions<ImagizeOptions>();
            }

            services.TryAddScoped<IExifTools, ExifRemover>();
            services.TryAddScoped<IHttpTools, HttpTools>();
            
            return services;
        }
    }

    public class ImagizeOptions
    {
        public string? AllowedOrigins { get; set; }
        public string? AllowedFileTypes { get; set; }
    }
}
