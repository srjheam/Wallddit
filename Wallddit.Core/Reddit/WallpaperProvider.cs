using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

using Wallddit.Core.Extensions;

namespace Wallddit.Core.Reddit
{
    public class WallpaperProvider
    {
        private readonly APIWrapper _apiWrapper;
        private readonly int _wallpapersPerCall;
        private readonly Queue<Wallpaper> _wallpapersHistory;

        public string SubredditSource { get; }

        public WallpaperProvider()
        {
            _apiWrapper = new APIWrapper();
            _wallpapersPerCall = 100;
            _wallpapersHistory = new Queue<Wallpaper>();

            SubredditSource = "walpaper";
        }

        public async Task<Wallpaper> GetFreshWallpaperAsync()
        {
            var callParameters = new Dictionary<string, string>
            {
                ["limit"] = _wallpapersPerCall.ToString()
            };

            dynamic redditLink = null;
            do
            {
                var apiResponse = await _apiWrapper.GetHotPostsAsync("wallpaper", callParameters);
                foreach (var link in apiResponse.data.children)
                {
                    if (!_wallpapersHistory.Select(x => x.Id).Contains((string)link.data.name))
                    {
                        redditLink = link.data;
                    }
                }
                callParameters.Add("after", (string)apiResponse.data.after);
            } while (redditLink == null);

            var wallpaper = GetWallpaperFromJson(redditLink);
            _wallpapersHistory.Enqueue(wallpaper);

            return wallpaper;
        }

        private Wallpaper GetWallpaperFromJson(dynamic json)
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

            var wallpaperId = (string)json.name;
            var wallpaperAuthor = (string)json.author;
            var wallpaperImage = new BitmapImage(wallpaperUri);

            var wallpaper = new Wallpaper(wallpaperId, wallpaperAuthor, wallpaperImage);
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
