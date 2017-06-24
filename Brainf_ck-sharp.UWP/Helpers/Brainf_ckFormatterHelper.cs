using System.Collections.Generic;
using Windows.UI;

namespace Brainf_ck_sharp_UWP.Helpers
{
    /// <summary>
    /// A small class with some UI-related methods for the Brainf_ck language
    /// </summary>
    public static class Brainf_ckFormatterHelper
    {
        /// <summary>
        /// Gets the syntax highlight colors map for the available operators
        /// </summary>
        private static readonly IReadOnlyDictionary<char, Color> HighlightMap = new Dictionary<char, Color>
        {
            { '>', Color.FromArgb(byte.MaxValue, 0xDD, 0xDD, 0xDD) },
            { '<', Color.FromArgb(byte.MaxValue, 0xDD, 0xDD, 0xDD) },
            { '+', Colors.White },
            { '-', Colors.White },
            { '[', Color.FromArgb(byte.MaxValue, 0x56, 0x9C, 0xD6) },
            { ']', Color.FromArgb(byte.MaxValue, 0x56, 0x9C, 0xD6) },
            { '.', Colors.IndianRed },
            { ',', Colors.DarkKhaki }
        };

        /// <summary>
        /// Returns the corresponding color from a given character in a Brainf_ck source code
        /// </summary>
        /// <param name="c">The character to parse</param>
        public static Color GetSyntaxHighlightColorFromChar(char c)
        {
            return HighlightMap.TryGetValue(c, out Color color) ? color : Color.FromArgb(byte.MaxValue, 0x52, 0xAF, 0x3D);
        }
    }
}
