namespace Imagize.Core
{
    public interface IHttpTools
    {
        Task<byte[]> DownloadAsync(string uri);

        bool IsValidOrigin(string uri, string imagizeOptionsAllowedOrigins);
        bool IsValidFileType(string uri, string imagizeOptionsAllowedFileTypes);
    }
}
