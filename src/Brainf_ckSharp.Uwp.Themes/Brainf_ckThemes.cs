using Windows.UI;
using Brainf_ckSharp.Uwp.Themes.Enums;

#nullable enable

namespace Brainf_ckSharp.Uwp.Themes;

/// <summary>
/// A <see langword="class"/> that exposes a collection of available themes
/// </summary>
public static class Brainf_ckThemes
{
    private static Brainf_ckTheme? visualStudio;

    /// <summary>
    /// Gets the Visual Studio theme
    /// </summary>
    public static Brainf_ckTheme VisualStudio
    {
        get
        {
            return visualStudio ??= new(
                0xFF1E1E1E.ToColor(),
                0xFF333333.ToColor(),
                0xFF237CAD.ToColor(),
                0xFF717171.ToColor(),
                3,
                0xFF52AF3D.ToColor(),
                0xFFDDDDDD.ToColor(),
                Colors.White,
                0xFF569CD6.ToColor(),
                Colors.IndianRed,
                Colors.DarkKhaki,
                0xFF009191.ToColor(),
                0xFF1E7499.ToColor(),
                LineHighlightStyle.Outline,
                0x30FFFFFF.ToColor(),
                "Visual Studio");
        }
    }

    private static Brainf_ckTheme? visualStudioCode;

    /// <summary>
    /// Gets the Visual Studio Code theme
    /// </summary>
    public static Brainf_ckTheme VisualStudioCode
    {
        get
        {
            return visualStudioCode ??= new(
                0xFF1E1E1E.ToColor(),
                0xFF252526.ToColor(),
                0xFF4B4B4B.ToColor(),
                0xFF404040.ToColor(),
                null,
                0xFF4B8B43.ToColor(),
                0xFF68D0FE.ToColor(),
                0xFFD4D4D4.ToColor(),
                0xFF31BEB0.ToColor(),
                0xFFC3622C.ToColor(),
                0xFFD0A641.ToColor(),
                0xFF437AC1.ToColor(),
                0xFF2F9187.ToColor(),
                LineHighlightStyle.Outline,
                0x20FFFFFF.ToColor(),
                "Visual Studio Code");
        }
    }

    private static Brainf_ckTheme? monokai;

    /// <summary>
    /// Gets the Monokai theme inspired to Atom and Sublime Text
    /// </summary>
    public static Brainf_ckTheme Monokai
    {
        get
        {
            return monokai ??= new(
                0xFF272822.ToColor(),
                0xFF49483E.ToColor(),
                0xFFA4A59E.ToColor(),
                0xFF474742.ToColor(),
                null,
                0xFFA6E22E.ToColor(),
                0xFF66D9EF.ToColor(),
                0xFFF8F8F2.ToColor(),
                0xFFFD971F.ToColor(),
                0xFFF92672.ToColor(),
                0xFFAE81FF.ToColor(),
                0xFFB56741.ToColor(),
                0xFF9B8FB2.ToColor(),
                LineHighlightStyle.Fill,
                0xFF515151.ToColor(),
                nameof(Monokai));
        }
    }

    private static Brainf_ckTheme? dracula;

    /// <summary>
    /// Gets the Dracula theme, ispired to the same theme available for Atom and Sublime Text
    /// </summary>
    public static Brainf_ckTheme Dracula
    {
        get
        {
            return dracula ??= new(
                0xFF282A36.ToColor(),
                0xFF414456.ToColor(),
                0xFFA5A5A6.ToColor(),
                Colors.LightGray,
                4,
                0xFF6272A4.ToColor(),
                0xFFF8F8F2.ToColor(),
                0xFFFF79C6.ToColor(),
                0xFF8BE9FD.ToColor(),
                0xFFFFB86C.ToColor(),
                0xFF50FA7B.ToColor(),
                0xFF527CFF.ToColor(),
                0xFF8C88CE.ToColor(),
                LineHighlightStyle.Fill,
                0xFF353746.ToColor(),
                nameof(Dracula));
        }
    }

    private static Brainf_ckTheme? vim;

    /// <summary>
    /// Gets the Vim from the old-school code editor
    /// </summary>
    public static Brainf_ckTheme Vim
    {
        get
        {
            return vim ??= new(
                0xFF171717.ToColor(),
                0xFF252525.ToColor(),
                0xFF727272.ToColor(),
                0xFF646464.ToColor(),
                6,
                0xFF99B96F.ToColor(),
                0xFFDFDFDF.ToColor(),
                0xFFDFDFDF.ToColor(),
                0xFF999BBC.ToColor(),
                0xFFFFB3B2.ToColor(),
                0xFFFFBC77.ToColor(),
                0xFF826880.ToColor(),
                0xFF917575.ToColor(),
                LineHighlightStyle.Fill,
                0xFF222222.ToColor(),
                nameof(Vim));
        }
    }

    private static Brainf_ckTheme? oneDark;

    /// <summary>
    /// Gets the Vim from the old-school code editor
    /// </summary>
    public static Brainf_ckTheme OneDark
    {
        get
        {
            return oneDark ??= new(
                0xFF282C34.ToColor(),
                0xFF383E49.ToColor(),
                0xFF5A5A5A.ToColor(),
                0xFF3C4049.ToColor(),
                null,
                0xFF5C6370.ToColor(),
                0xFFC0C0C0.ToColor(),
                0xFF56B6C2.ToColor(),
                0xFFABB2BF.ToColor(),
                0xFFD19A66.ToColor(),
                0xFFE06C75.ToColor(),
                0xFFBC794F.ToColor(),
                0xFFB5544C.ToColor(),
                LineHighlightStyle.Fill,
                0xFF363A4F.ToColor(),
                "One Dark");
        }
    }

    private static Brainf_ckTheme? xCodeDark;

    /// <summary>
    /// Gets the XCode Dark theme
    /// </summary>
    public static Brainf_ckTheme XCodeDark
    {
        get
        {
            return xCodeDark ??= new(
                0xFF292A30.ToColor(),
                0xFF383E49.ToColor(),
                0xFF5C5F62.ToColor(),
                0xFF393A3B.ToColor(),
                null,
                0xFF7F8C99.ToColor(),
                0xFF49B0CE.ToColor(),
                0xFFFCFCFC.ToColor(),
                0xFFFB79B0.ToColor(),
                0xFFCD9764.ToColor(),
                0xFFFF806C.ToColor(),
                0xFFB37EEE.ToColor(),
                0xFF855EB2.ToColor(),
                LineHighlightStyle.Fill,
                0xFF2F3239.ToColor(),
                "XCode Dark");
        }
    }

    private static Brainf_ckTheme? base16;

    /// <summary>
    /// Gets the Vim from the old-school code editor
    /// </summary>
    public static Brainf_ckTheme Base16
    {
        get
        {
            return base16 ??= new(
                0xFF1D1F21.ToColor(),
                0xFF373B41.ToColor(),
                0xFF656767.ToColor(),
                0xFF373B41.ToColor(),
                null,
                0xFF969896.ToColor(),
                0xFFE0935A.ToColor(),
                0xFFC5C8C6.ToColor(),
                0xFFB393BC.ToColor(),
                0xFFB5BE63.ToColor(),
                0xFFCE6564.ToColor(),
                0xFF357199.ToColor(),
                0xFF69789E.ToColor(),
                LineHighlightStyle.Fill,
                0xFF2E3032.ToColor(),
                "Base 16");
        }
    }

    /// <summary>
    /// Converts a given hex color into a <see cref="Color"/> value.
    /// </summary>
    /// <param name="hex">The input color.</param>
    /// <returns>The resulting <see cref="Color"/> value.</returns>
    private static Color ToColor(this int hex)
    {
        return ToColor((uint)hex);
    }

    /// <summary>
    /// Converts a given hex color into a <see cref="Color"/> value.
    /// </summary>
    /// <param name="hex">The input color.</param>
    /// <returns>The resulting <see cref="Color"/> value.</returns>
    private static Color ToColor(this uint hex)
    {
        byte a = (byte)(hex >> 24);
        byte r = (byte)((hex >> 16) & 0xff);
        byte g = (byte)((hex >> 8) & 0xff);
        byte b = (byte)(hex & 0xff);

        return Color.FromArgb(a, r, g, b);
    }
}
