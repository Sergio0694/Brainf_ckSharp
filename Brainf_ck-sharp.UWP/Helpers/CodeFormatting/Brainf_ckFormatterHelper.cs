using Windows.UI;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using Brainf_ck_sharp_UWP.DataModels.Misc.Themes;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Helpers.CodeFormatting
{
    /// <summary>
    /// A small class with some UI-related methods for the Brainf_ck language
    /// </summary>
    public sealed class Brainf_ckFormatterHelper
    {
        // Private constructor that loads the initial theme
        private Brainf_ckFormatterHelper() => CurrentTheme = CodeThemes.Default;

        private static Brainf_ckFormatterHelper _Instance;

        /// <summary>
        /// Gets the singleton instance of the class with the current theme ready to use
        /// </summary>
        [NotNull]
        public static Brainf_ckFormatterHelper Instance => _Instance ?? (_Instance = new Brainf_ckFormatterHelper());

        /// <summary>
        /// Gets the syntax highlight colors map for the available operators
        /// </summary>
        [NotNull]
        public IDEThemeInfo CurrentTheme { get; private set; }

        /// <summary>
        /// Checks whether or not two operators have the same highlighted color
        /// </summary>
        /// <param name="first">The first operator</param>
        /// <param name="second">The second operator</param>
        public static bool HaveSameColor(char first, char second) => first == second ||
                                                                     first == '>' && second == '<' ||
                                                                     first == '<' && second == '>' ||
                                                                     first == '+' && second == '-' ||
                                                                     first == '-' && second == '+' ||
                                                                     first == '[' && second == ']' ||
                                                                     first == ']' && second == '[';

        /// <summary>
        /// Returns the corresponding color from a given character in a Brainf_ck source code
        /// </summary>
        /// <param name="c">The character to parse</param>
        public Color GetSyntaxHighlightColorFromChar(char c)
        {
            return CurrentTheme.HighlightMap.TryGetValue(c, out Color color) ? color : CurrentTheme.CommentsColor;
        }
    }
}
