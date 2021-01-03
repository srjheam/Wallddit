using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Wallddit.Core.Services;

namespace Wallddit.Core.Reddit
{
    /// <summary>
    /// This class is the simplest kind of wrapper for the Reddit API.
    /// </summary>
    /// <remarks>
    /// For more information about the Reddit API, access: https://www.reddit.com/dev/api
    /// </remarks>
    public class APIWrapper
    {
        /// <value>
        /// Gets the base URI for any request, in this case it's <c>"https://www.reddit.com"</c>.
        /// </value>
        private readonly Uri _baseUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="APIWrapper"/> class.
        /// </summary>
        public APIWrapper() =>
            _baseUri = new Uri("https://www.reddit.com/");

        /// <summary>
        /// Send a GET request for hot posts on either reddit home page or a subreddit and return the response body as a dynamic object in an asynchronous operation.
        /// </summary>
        /// <param name="subreddit">(optional) The subreddit from which the hot posts will be obtained.</param>
        /// <param name="parameters">(optional) The parameters that will be passed along with the request.</param>
        /// <returns>The response body as a JSON in a dynamic object.</returns>
        public async Task<dynamic> GetHotPostsAsync(string subreddit = null, Dictionary<string, string> parameters = null)
        {
            var relativeUri = "hot.json";
            if (!String.IsNullOrWhiteSpace(subreddit))
            {
                relativeUri = String.Concat("r/", subreddit, '/', relativeUri);
            }

            var requestUri = String.Concat(_baseUri, relativeUri);

            requestUri = AddParamatersToUriQuery(requestUri, parameters);

            return await HttpDataService.GetAsync<dynamic>(requestUri).ConfigureAwait(false);
        }

        /// <summary>
        /// Add parameters to the uri query.
        /// </summary>
        /// <param name="uri">The Uri the parameters is add to.</param>
        /// <param name="parameters">The parameters to be add.</param>
        /// <returns>The Uri with the new parameters.</returns>
        private string AddParamatersToUriQuery(string uri, Dictionary<string, string> parameters)
        {
            if (!(parameters == null || parameters.Count == 0))
            {
                if (!uri.Contains('?'))
                {
                    uri += '?';
                }
                else if (!uri.EndsWith('?'))
                {
                    uri += '&';
                }

                uri += String.Join('&', Enumerable.Zip(parameters.Keys, parameters.Values, (p, v) => $"{p}={v}"));
            }

            return uri;
        }
    }
}
