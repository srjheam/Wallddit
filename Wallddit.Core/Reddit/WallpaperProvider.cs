using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Wallddit.Core.Extensions;
using Wallddit.Core.Models;
using Wallddit.Core.Services;

namespace Wallddit.Core.Reddit
{
    public class WallpaperProvider
    {
        private readonly APIWrapper _apiWrapper;
        private readonly int _wallpapersPerCall;
        private readonly SqliteDataService _wallpaperDb;

        public string SubredditSource { get; }

        public WallpaperProvider(string wallpaperDBPath)
        {
            _apiWrapper = new APIWrapper();
            _wallpapersPerCall = 100;
            _wallpaperDb = new SqliteDataService(wallpaperDBPath);

            SubredditSource = "wallpaper";
        }

        public async Task<Wallpaper> GetWallpaperAsync()
        {
            var callParameters = new Dictionary<string, string>
            {
                ["limit"] = _wallpapersPerCall.ToString()
            };

            dynamic redditLink = null;
            do
            {
                var apiResponse = await _apiWrapper.GetHotPostsAsync(SubredditSource, callParameters);
                foreach (var link in apiResponse.data.children)
                {
                    if (!(await _wallpaperDb.ReadAllWallpapersAsync()).Select(x => x.Id).Contains((string)link.data.name))
                    {
                        redditLink = link.data;
                        break;
                    }
                }
                callParameters.Add("after", (string)apiResponse.data.after);
            } while (redditLink == null);

            Wallpaper wallpaper = ParseWallpaperFromJson(redditLink);
            await _wallpaperDb.CreateWallpaperAsync(wallpaper);

            return wallpaper;
        }

        private Wallpaper ParseWallpaperFromJson(dynamic json)
        {
            Uri wallpaperUri;
            try
            {
                wallpaperUri = GetWallpaperUriFromGallery(json, 0);
            }
            catch (Exception)
            {
                wallpaperUri = new Uri((string)json.url);
            }

            var wallpaper = new Wallpaper
            {
                Id = (string)json.name,
                Title = (string)json.title,
                Author = (string)json.author,
                IsSaved = false,
                AuthorProfileUrl = $"https://www.reddit.com/user/{(string)json.author}/",
                ImageUrl = wallpaperUri.AbsoluteUri,
                SourceUrl = $"https://www.reddit.com{(string)json.permalink}",
                Provider = "Reddit"
            };

            return wallpaper;
        }

        private Uri GetWallpaperUriFromGallery(dynamic redditLink, int index)
        {
            var wallpaperId = (string)redditLink.gallery_data.items[index].media_id;
            var wallpaperUri = $"https://i.redd.it/{wallpaperId}.png";
            return wallpaperUri.ToUri();
        }
    }
}
