using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Wallddit.Core.Models
{
    public class Wallpaper : ReactiveObject
    {
        /// <summary>
        /// Gets the Wallpaper unique identificator
        /// </summary>
        [Reactive]
        public string Id { get; set; }
        /// <summary>
        /// Gets the Wallpaper post title.
        /// </summary>
        [Reactive]
        public string Title { get; set; }
        /// <summary>
        /// Gets the name of the Wallpaper author.
        /// </summary>
        [Reactive]
        public string Author { get; set; }
        /// <summary>
        /// Gets a boolean briefing whether the wallpaper is marked as saved or not.
        /// </summary>
        [Reactive]
        public bool IsSaved { get; set; }
        /// <summary>
        /// Gets the url to the author profile.
        /// </summary>
        [Reactive]
        public string AuthorProfileUrl { get; set; }
        /// <summary>
        /// Gets the Wallpaper image Url.
        /// </summary>
        [Reactive]
        public string ImageUrl { get; set; }
        /// <summary>
        /// Gets the Wallpaper source url.
        /// </summary>
        [Reactive]
        public string SourceUrl { get; set; }
        /// <summary>
        /// Gets the Wallpaper Provider.
        /// </summary>
        [Reactive]
        public string Provider { get; set; }
    }
}
