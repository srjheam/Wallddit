using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Wallddit.Core.Services
{
    /// <summary>
    /// This service class is used for sending HTTP requests and receiving HTTP responses from a resource identified by a URI.
    /// </summary>
    public static class HttpDataService
    {
        private static readonly HttpClient _client = new HttpClient();

        /// <summary>
        /// Send a GET request to the specified Uri and return a deserialized object from the response body in an asynchronous operation.
        /// </summary>
        /// <typeparam name="T">The object the response body will be deserialized in.</typeparam>
        /// <param name="uri">The Uri the request is sent to.</param>
        /// <param name="accessToken">(optional) The token to be passed along the GET request.</param>
        /// <returns>The deserialized object from the response body.</returns>
        /// <exception cref="JsonReaderException">Thrown when the request response body isn't a valid JSON.</exception>
        public static async Task<T> GetAsync<T>(string uri, string accessToken = null)
        {
            T result = default;

            AddAuthorizationHeader(accessToken);
            var json = await _client.GetStringAsync(uri).ConfigureAwait(false);
            result = await Task.Run(() => JsonConvert.DeserializeObject<T>(json)).ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// Downloads an image asynchronously from the <paramref name="uri"/> and places it in the specified <paramref name="directoryPath"/> with the specified <paramref name="fileName"/>.
        /// </summary>
        /// <param name="directoryPath">The relative or absolute path to the directory to place the image in.</param>
        /// <param name="fileName">The name of the file without the file extension.</param>
        /// <param name="uri">The URI for the image to download.</param>
        /// <param name="forceDownload">(optional) True if the image should be downloaded, regardless of whether it has already been downloaded.</param>
        /// <param name="accessToken">(optional) The token to be passed along the GET request.</param>
        /// <returns>Returns the absolute path to the downloaded image.</returns>
        public static async Task<string> DownloadImageAsync(string directoryPath, string fileName, Uri uri, bool forceDownload = false, string accessToken = null)
        {
            /*
             * Thanks to MarcusOtter for providing the code
             * used in DownloadImageAsync.
             * See it at: https://gist.github.com/MarcusOtter/b9b4ee3fc7be04469fd20480daa86c38
             */

            AddAuthorizationHeader(accessToken);

            // Get the file extension
            var uriWithoutQuery = uri.GetLeftPart(UriPartial.Path);
            var fileExtension = Path.GetExtension(uriWithoutQuery);

            // Create file path and ensure directory exists
            var path = Path.Combine(directoryPath, $"{fileName}{fileExtension}");
            Directory.CreateDirectory(directoryPath);

            // Check if the image is already downloaded and avoid downloading it again
            if (forceDownload || !File.Exists(path))
            {
                // Download the image and write to the file
                var imageBytes = await _client.GetByteArrayAsync(uri);
                await File.WriteAllBytesAsync(path, imageBytes);
            }

            return path;
        }

        private static void AddAuthorizationHeader(string token) =>
            _client.DefaultRequestHeaders.Authorization = String.IsNullOrWhiteSpace(token) ? null : new AuthenticationHeaderValue("Bearer", token);
    }
}
