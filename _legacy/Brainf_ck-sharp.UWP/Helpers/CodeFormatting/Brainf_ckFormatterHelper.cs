using Windows.UI;
using Brainf_ck_sharp_UWP.DataModels.Misc.Themes;
using Brainf_ck_sharp_UWP.Helpers.Settings;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Helpers.CodeFormatting
{
    /// <summary>
    /// A small class with some UI-related methods for the Brainf_ck language
    /// </summary>
    public sealed class Brainf_ckFormatterHelper
    {
        // Private constructor that loads the initial theme
        private Brainf_ckFormatterHelper() => CurrentTheme = LoadTheme();

        private static Brainf_ckFormatterHelper _Instance;

        /// <summary>
        /// Gets the singleton instance of the class with the current theme ready to use
        /// </summary>
        [NotNull]
        public static Brainf_ckFormatterHelper Instance => _Instance ?? (_Instance = new Brainf_ckFormatterHelper());

        // Parses the theme in use from the settings
        private IDEThemeInfo LoadTheme()
        {
            int theme = AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.SelectedIDETheme));
            switch (theme)
            {
                case 1: return CodeThemes.Monokai;
                case 2: return CodeThemes.Dracula;
                case 3: return CodeThemes.Vim;
                case 4: return CodeThemes.OneDark;
                case 5: return CodeThemes.Base16;
                case 6: return CodeThemes.VisualStudioCode;
                default: return CodeThemes.Default;
            }
        }

        /// <summary>
        /// Loads the current IDE theme in use from the local settings
        /// </summary>
        public void ReloadTheme() => CurrentTheme = LoadTheme();

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
                                                                     first == ']' && second == '[' ||
                                                                     first == '(' && second == ')' ||
                                                                     first == ')' && second == '(';

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
