using Imagize.Abstractions;
using Imagize.Core;
using Imagize.Providers.SkiaSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Imagize.Providers.SkiaSharp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddImagizeSkiaSharp(this IServiceCollection services,
            Action<ImagizeSkiaSharpOptions>? imagizeSkiaSharpOptionsAction = null)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            // Setup Options...
            if (imagizeSkiaSharpOptionsAction is not null)
            {
                // Build a new configuration into which we'll merge the config supplied
                ImagizeSkiaSharpOptions? imagizeSkiaSharpOptions = new();

                // Merge the configuration (if one is supplied)
                imagizeSkiaSharpOptionsAction?.Invoke(imagizeSkiaSharpOptions);

                services.AddOptions<ImagizeSkiaSharpOptions>()
                    .Configure(imagizeSkiaSharpOptionsAction);
            }
            else
            {
                services.AddOptions<ImagizeSkiaSharpOptions>();
            }
            services.TryAddScoped<IImageTools, ImageTools>();
            return services;
        }
    }

    public class ImagizeSkiaSharpOptions
    {
        
    }
}