using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

using Wallddit.Core.Extensions;

namespace Wallddit.Core.Reddit
{
    public class WallpaperProvider
    {
        private readonly APIWrapper _apiWrapper;

        public string SubredditSource { get; }

        public WallpaperProvider()
        {
            _apiWrapper = new APIWrapper();
            SubredditSource = "walpaper"; ;
        }

        public async Task<Wallpaper> GetFreshWallpaperAsync()
        {
            var apiResponse = await _apiWrapper.GetHotPostsAsync("wallpaper", new Dictionary<string, string>
            {
                ["limit"] = "1"
            });
            dynamic redditLink = apiResponse.data.children[0].data;

            Uri wallpaperUri;
            try
            {
                wallpaperUri = GetWallpaperUriFromGallery(redditLink, 0);
            }
            catch (Exception)
            {
                wallpaperUri = new Uri((string)redditLink.url);
            }

            var wallpaperId = (string)redditLink.name;
            var wallpaperAuthor = (string)redditLink.author;
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
