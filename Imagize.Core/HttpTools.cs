namespace Imagize.Core
{
    public class HttpTools : IHttpTools
    {
        private readonly HttpClient _httpClient;
        private char separator = '|';

        public HttpTools(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Download bytes from a Url
        /// Just returns a zero byte array if there is a fault. eg 404 etc.
        /// </summary>
        /// <param name="uri">The uri of the object to download</param>
        /// <returns>byte[]</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<byte[]> DownloadAsync(string uri)
        {
            if (!Uri.TryCreate(uri, UriKind.Absolute, out Uri? _))
                return new byte[] { };
                // throw new InvalidOperationException("URI is invalid.");

            byte[] result;
            try
            {
                result = await _httpClient.GetByteArrayAsync(uri);
            }
            catch (Exception)
            {
                return new byte[] {}; // Just return a zero byte array if fault
            }

            return result;

        }

        /// <summary>
        /// Returns whether or not a Uri is allowed
        /// Will also match the first level of path segment if exists in allowed origins
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="imagizeOptionsAllowedOrigins"></param>
        /// <returns>boolean</returns>
        public bool IsValidOrigin(string uri, string imagizeOptionsAllowedOrigins)
        {
            if (!Uri.TryCreate(uri, UriKind.Absolute, out Uri? _))
                return false; // throw new InvalidOperationException("URI is invalid.");
            
            if (string.IsNullOrEmpty(imagizeOptionsAllowedOrigins))
                return true;

            string[] allowedOrigins = imagizeOptionsAllowedOrigins.Split(separator);

            if (allowedOrigins.Length == 0)
                return true;

            Uri uriToCheck = new(uri);

            foreach (string origin in allowedOrigins)
            {

                if (!Uri.TryCreate(origin, UriKind.Absolute, out Uri? validOriginUri))
                    continue; // Ignore invalid URIs

                // If the origin has a path part, grab the first segment and check that the host and that segment match
                if (validOriginUri.Segments.Length > 1)
                {
                    // Check if both the host and the first (real , eg not the '/' which is segment 0) match
                    // see: https://docs.microsoft.com/en-us/dotnet/api/system.uri?view=net-6.0
                    // eg for.. Uri uri = new Uri("https://user:password@www.contoso.com:80/Home/Index.htm?q1=v1&q2=v2#FragmentName");
                    // Console.WriteLine($"Segments: {string.Join(", ", uri.Segments)}");
                    // returns..
                    //           0    1      2
                    // Segments: /, Home/, Index.htm

                    if (uriToCheck.Host == validOriginUri.Host && uriToCheck.Segments[1].Trim('/') == validOriginUri.Segments[1].Trim('/'))
                        return true;
                }
                else
                {
                    // Otherwise do the normal check of just the host
                    if (Uri.Compare(uriToCheck, validOriginUri,
                            UriComponents.Host, // Check the host only
                            UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) == 0)
                        return true;
                }

              
            }

            return false;
        }

        /// <summary>
        /// Check for valid file type
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="imagizeOptionsAllowedFileTypes"></param>
        /// <returns></returns>
        public bool IsValidFileType(string uri, string imagizeOptionsAllowedFileTypes)
        {
            if (!Path.HasExtension(uri))
                return false;

            string[] allowedFileTypes = imagizeOptionsAllowedFileTypes.Split(separator);

            if (allowedFileTypes.Length == 0)
                return true; // Any file type is allowed as no restrictions were set.

            string extension = Path.GetExtension(uri).Trim('.');

            foreach (string allowedFileType in allowedFileTypes)
            {
                if (string.Equals(extension, allowedFileType, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}
