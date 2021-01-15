using Windows.UI.Xaml.Media.Imaging;

namespace Wallddit.Core
{
    public class Wallpaper
    {
        /// <summary>
        /// Gets the Wallpaper's unique identificator
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// Gets the author of the Wallpaper.
        /// </summary>
        public string Author { get; }
        /// <summary>
        /// Gets the Wallpaper's image.
        /// </summary>
        public BitmapImage Image { get; }

        public Wallpaper(string id, string author, BitmapImage image)
        {
            Id = id;
            Author = author;
            Image = image;
        }
    }
}
