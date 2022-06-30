using System.Text.Json.Serialization;

namespace Imagize.Abstractions
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ImageFormat
    {
        BMP,
        JPEG,
        GIF,
        TIFF,
        PNG,
        UNKNOWN
    }
}
