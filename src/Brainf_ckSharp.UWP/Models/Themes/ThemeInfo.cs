using System.Collections.Generic;
using Windows.UI;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.UWP.Models.Themes.Enums;

namespace Brainf_ckSharp.UWP.Models.Themes
{
    /// <summary>
    /// A model that that holds all the relevant color info on a given formatting theme
    /// </summary>
    public sealed class ThemeInfo
    {
        /// <summary>
        /// Gets the name of the current theme
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the syntax highlight colors map for the available operators
        /// </summary>
        public IReadOnlyDictionary<char, Color> HighlightMap { get; }

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
        /// Creates a new <see cref="ThemeInfo"/> instance with the specified parameters
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
        public ThemeInfo(
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
            LineHighlightStyle = lineStyle;
            LineHighlightColor = lineColor;
            HighlightMap = new Dictionary<char, Color>
            {
                [Characters.BackwardPtr] = arrows,
                [Characters.BackwardPtr] = arrows,
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
        }
    }
}
