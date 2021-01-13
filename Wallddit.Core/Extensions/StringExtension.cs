using System;

namespace Wallddit.Core.Extensions
{
    /// <summary>
    /// Provide some string extensions.
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// Convert the <paramref name="str"/> to its equivalent <see cref="Uri"/>.
        /// </summary>
        /// <param name="str">The string to convert.</param>
        /// <returns>The equivalent <see cref="Uri"/> representation of the <paramref name="str"/> string.</returns>
        public static Uri ToUri(this string str)
        {
            var uri = new Uri(str);
            return uri;
        }
    }
}
