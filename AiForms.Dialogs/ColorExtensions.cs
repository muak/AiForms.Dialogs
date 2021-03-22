using System;
using Xamarin.Forms;
namespace AiForms.Dialogs.Abstractions
{
    /// <summary>
    /// Color extensions.
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Ises the transparent or default.
        /// </summary>
        /// <returns><c>true</c>, if transparent or default was ised, <c>false</c> otherwise.</returns>
        /// <param name="color">Color.</param>
        public static bool IsTransparentOrDefault(this Color color)
        {
            return color.IsDefault || color == Color.Transparent;
        }
    }
}
