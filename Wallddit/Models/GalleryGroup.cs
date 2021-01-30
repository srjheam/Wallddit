using System.Collections.Generic;

using Wallddit.Core.Models;

namespace Wallddit.Models
{
    internal class GalleryGroup
    {
        public string Title { get; set; }
        public List<Wallpaper> Wallpapers { get; set; }
    }
}
