using System.Text.Json.Serialization;

namespace Imagize.Abstractions
{

    [JsonConverter(typeof(JsonStringEnumConverter))]    
    public enum ImageQuality
    {
        None,
        Low,
        Medium,
        High
    }
}
