namespace Imagize.Abstractions
{
    public class ImagizeOptions
    {
        public string? AllowedOrigins { get; set; }
        public string? AllowedFileTypes { get; set; }
        public TextSupport TextSupport { get; set; } = TextSupport.Default;
    }
}
