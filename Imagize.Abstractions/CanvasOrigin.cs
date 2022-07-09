using System.Text.Json.Serialization;

namespace Imagize.Abstractions
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CanvasOrigin
    {
        TopLeft,
        BottomLeft
    }
}
