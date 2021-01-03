using Newtonsoft.Json;
using System;
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
        private static readonly HttpClient client = new HttpClient();

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
            var json = await client.GetStringAsync(uri).ConfigureAwait(false);
            result = await Task.Run(() => JsonConvert.DeserializeObject<T>(json)).ConfigureAwait(false);

            return result;
        }

        private static void AddAuthorizationHeader(string token) =>
            client.DefaultRequestHeaders.Authorization = String.IsNullOrWhiteSpace(token) ? null : new AuthenticationHeaderValue("Bearer", token);
    }
}
