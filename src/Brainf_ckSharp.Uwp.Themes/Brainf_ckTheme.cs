using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Uwp.Themes.Enums;

namespace Brainf_ckSharp.Uwp.Themes;

/// <summary>
/// A model that that holds all the relevant color info on a given formatting theme
/// </summary>
public sealed class Brainf_ckTheme
{
    /// <summary>
    /// The syntax highlight brushes map for the available operators
    /// </summary>
    /// <remarks>Using a <see cref="Dictionary{TKey,TValue}"/> to try to avoid the <see langword="callvirt"/> call when retrieving values</remarks>
    private readonly Dictionary<char, SolidColorBrush> HighlightBrushMap;

    /// <summary>
    /// Gets the syntax highlight colors map for the available operators
    /// </summary>
    private readonly Dictionary<char, Color> HighlightColorMap;

    /// <summary>
    /// The brush of the comments in the code
    /// </summary>
    private readonly Brush CommentsBrush;

    /// <summary>
    /// Creates a new <see cref="Brainf_ckTheme"/> instance with the specified parameters
    /// </summary>
    /// <param name="background">The IDE background color</param>
    /// <param name="breakpoints">The breakpoints pane background color</param>
    /// <param name="lineNumbers">The line indicators foreground color</param>
    /// <param name="bracketsGuide">The vertical guides color</param>
    /// <param name="bracketsGuideStrokesLength">The optional stroke length of the vertical guides</param>
    /// <param name="comments">The comments color</param>
    /// <param name="arrows">The foreground color for the &gt; and &lt; operators</param>
    /// <param name="arithmetic">The foreground color for the + and - operators</param>
    /// <param name="brackets">The foreground color for the square brackets</param>
    /// <param name="dot">The foreground color for the dot operator</param>
    /// <param name="comma">The foreground color for the comma operator</param>
    /// <param name="function"></param>
    /// <param name="call"></param>
    /// <param name="lineStyle">The line highlight style</param>
    /// <param name="lineColor">The color of the line highlight</param>
    /// <param name="name">The name of the new theme to create</param>
    public Brainf_ckTheme(
        Color background,
        Color breakpoints,
        Color lineNumbers,
        Color bracketsGuide,
        int? bracketsGuideStrokesLength,
        Color comments,
        Color arrows,
        Color arithmetic,
        Color brackets,
        Color dot,
        Color comma,
        Color function,
        Color call,
        LineHighlightStyle lineStyle,
        Color lineColor,
        string name)
    {
        Name = name;
        Background = background;
        BreakpointsPaneBackground = breakpoints;
        LineNumberColor = lineNumbers;
        BracketsGuideColor = bracketsGuide;
        BracketsGuideStrokesLength = bracketsGuideStrokesLength;
        CommentsColor = comments;
        CommentsBrush = new SolidColorBrush(comments);
        LineHighlightStyle = lineStyle;
        LineHighlightColor = lineColor;
        HighlightColorMap = new Dictionary<char, Color>
        {
            [Characters.BackwardPtr] = arrows,
            [Characters.ForwardPtr] = arrows,
            [Characters.Plus] = arithmetic,
            [Characters.Minus] = arithmetic,
            [Characters.PrintChar] = dot,
            [Characters.ReadChar] = comma,
            [Characters.LoopStart] = brackets,
            [Characters.LoopEnd] = brackets,
            [Characters.FunctionStart] = function,
            [Characters.FunctionEnd] = function,
            [Characters.FunctionCall] = call
        };
        HighlightBrushMap = HighlightColorMap.ToDictionary(p => p.Key, p => new SolidColorBrush(p.Value));
    }

    /// <summary>
    /// Gets the name of the current theme
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the color of the comments in the code
    /// </summary>
    public Color CommentsColor { get; }

    /// <summary>
    /// Gets the background color of the current theme
    /// </summary>
    public Color Background { get; }

    /// <summary>
    /// Gets the color of the pane where the breakpoints are displayed
    /// </summary>
    public Color BreakpointsPaneBackground { get; }

    /// <summary>
    /// Gets the color of the line number indicators
    /// </summary>
    public Color LineNumberColor { get; }

    /// <summary>
    /// Gets the color of the vertical brackets guides
    /// </summary>
    public Color BracketsGuideColor { get; }

    /// <summary>
    /// Gets the length of each stroke in the vertical brackets guides, if present
    /// </summary>
    public int? BracketsGuideStrokesLength { get; }

    /// <summary>
    /// Gets the visual style for the line highlight
    /// </summary>
    public LineHighlightStyle LineHighlightStyle { get; }

    /// <summary>
    /// Gets the color for the line highlight
    /// </summary>
    public Color LineHighlightColor { get; }

    /// <summary>
    /// Checks whether or not two operators have the same highlighted color
    /// </summary>
    /// <param name="first">The first operator</param>
    /// <param name="second">The second operator</param>
    public static bool HaveSameColor(char first, char second)
    {
        if (first == second) return true;

        // Always have the lowest character in first position
        if (second > first) (first, second) = (second, first);

        return
            !Brainf_ckParser.IsOperator(first) && !Brainf_ckParser.IsOperator(second) ||
            first == Characters.BackwardPtr && second == Characters.ForwardPtr ||
            first == Characters.Plus && second == Characters.Minus ||
            first == Characters.LoopStart && second == Characters.LoopEnd ||
            first == Characters.FunctionStart && second == Characters.FunctionEnd;
    }

    /// <summary>
    /// Gets the corresponding <see cref="Color"/> from a given character in a Brainf*ck/PBrain source code
    /// </summary>
    /// <param name="c">The character to parse</param>
    /// <returns>The <see cref="Color"/> value for the input character</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Color GetColor(char c) => HighlightColorMap.TryGetValue(c, out Color color) ? color : CommentsColor;

    /// <summary>
    /// Gets the corresponding <see cref="Brush"/> from a given character in a Brainf*ck/PBrain source code
    /// </summary>
    /// <param name="c">The character to parse</param>
    /// <returns>The <see cref="Brush"/> value for the input character</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Brush GetBrush(char c) => HighlightBrushMap.TryGetValue(c, out SolidColorBrush brush) ? brush : CommentsBrush;
}
