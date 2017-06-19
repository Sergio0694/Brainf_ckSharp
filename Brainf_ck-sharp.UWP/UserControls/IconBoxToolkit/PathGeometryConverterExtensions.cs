using System;
using Windows.UI.Xaml.Media;

namespace Brainf_ck_sharp_UWP.UserControls.IconBoxToolkit
{
    /// <summary>
    /// A path geometry converter, credits to Rafael Yousuf
    /// </summary>
    public static class PathGeometryConverterExtensions
    {
        /// <summary>
        /// Parses the specified path data.
        /// </summary>
        /// <param name="pathData">The path data.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static PathGeometry Parse(this string pathData)
        {
            if (string.IsNullOrWhiteSpace(pathData))
                throw new ArgumentNullException(nameof(pathData));

            var converter = new PathGeometryConverter();
            var pathGeometry = converter.Convert(pathData);

            return pathGeometry;
        }
    }
}
