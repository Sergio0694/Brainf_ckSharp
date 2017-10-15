using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using SharpDX;
using SharpDX.DirectWrite;

namespace Brainf_ck_sharp_UWP.Helpers.CodeFormatting
{
    /// <summary>
    /// A class that represents an installed font on the current device
    /// </summary>
    public class InstalledFont
    {
        /// <summary>
        /// Gets the name of the font
        /// </summary>
        [NotNull]
        public String Name { get; }

        private Windows.UI.Xaml.Media.FontFamily _Family;

        /// <summary>
        /// Gets the family of font to use in a text control
        /// </summary>
        public Windows.UI.Xaml.Media.FontFamily Family => _Family ?? (_Family = new Windows.UI.Xaml.Media.FontFamily(Name));

        private static IReadOnlyList<InstalledFont> _Fonts;

        /// <summary>
        /// Gets the list of available fonts on the current device
        /// </summary>
        [NotNull]
        public static IReadOnlyList<InstalledFont> Fonts
        {
            get
            {
                // Return the current fonts list or try to generate one
                if (_Fonts != null) return _Fonts;
                try
                {
                    return _Fonts = GetFonts();
                }
                catch (SharpDXException)
                {
                    // Internal library exception, return the default font for now
                    return new[] { new InstalledFont(DefaultSegoeFont) };
                }
            }
        }

        /// <summary>
        /// Tries to retrieve an <see cref="InstalledFont"/> with the given name
        /// </summary>
        /// <param name="name">The name of the font to retrieve</param>
        /// <param name="font">The resulting <see cref="InstalledFont"/> in case of success, null otherwise</param>
        public static bool TryGetFont([NotNull] String name, out InstalledFont font)
        {
            font = Fonts.FirstOrDefault(f => f.Name.Equals(name));
            return font != null;
        }

        // Private constructor
        private InstalledFont([NotNull] String name)
        {
            Name = name;
        }

        /// <summary>
        /// Returns a list of the available font on the current device
        /// </summary>
        [NotNull]
        private static IReadOnlyList<InstalledFont> GetFonts()
        {
            // Prepare the loop
            List<InstalledFont> fontList = new List<InstalledFont>();
            Factory factory = new Factory();
            FontCollection fontCollection = factory.GetSystemFontCollection(false);
            int familyCount = fontCollection.FontFamilyCount;

            // Iterate over the available fonts
            for (int i = 0; i < familyCount; i++)
            {
                // Get the current font family
                FontFamily fontFamily = fontCollection.GetFontFamily(i);
                LocalizedStrings familyNames = fontFamily.FamilyNames;

                // Get the local name of the font
                if (!familyNames.FindLocaleName(CultureInfo.CurrentCulture.Name, out int index))
                {
                    familyNames.FindLocaleName("en-us", out index);
                }

                // Gets the actual font family name to return
                String name = familyNames.GetString(index);
                if (AllowedFonts.Contains(name)) fontList.Add(new InstalledFont(name));
            }
            return fontList.OrderBy(font => font.Name).ToArray();
        }

        // Gets the default Segoe font name
        private const String DefaultSegoeFont = "Segoe UI";

        /// <summary>
        /// Gets the list of allowed font families
        /// </summary>
        [NotNull]
        private static readonly ISet<String> AllowedFonts = new HashSet<String>
        {
            "Calibri",
            "Cambria",
            "Consolas",
            DefaultSegoeFont
        };
    }
}
