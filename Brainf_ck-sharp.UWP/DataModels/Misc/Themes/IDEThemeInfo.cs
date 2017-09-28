using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Windows.UI;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.DataModels.Misc.Themes
{
    /// <summary>
    /// A class that holds all the relevant color info on an IDE theme
    /// </summary>
    public sealed class IDEThemeInfo
    {
        /// <summary>
        /// Gets the name of the current theme
        /// </summary>
        [NotNull]
        public String Name { get; }

        /// <summary>
        /// Gets the syntax highlight colors map for the available operators
        /// </summary>
        [NotNull]
        public IReadOnlyDictionary<char, Color> HighlightMap { get; }

        /// <summary>
        /// Gets the color of the comments in the code
        /// </summary>
        public Color CommentsColor { get; }

        /// <summary>
        /// Gets the background color of the IDE
        /// </summary>
        public Color Background { get; }

        /// <summary>
        /// Gets the color of the left pane where the breakpoints are displayed
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
        /// Creates a new theme with the given parameters
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
        public IDEThemeInfo(
            Color background, Color breakpoints, Color lineNumbers, 
            Color bracketsGuide, int? bracketsGuideStrokesLength, 
            Color comments, Color arrows, Color arithmetic, Color brackets, Color dot, Color comma, Color function, Color call, 
            LineHighlightStyle lineStyle, Color lineColor, [CallerMemberName] string name = null)
        {
            Name = name ?? throw new NullReferenceException("Invalid theme name");
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
                { '>', arrows },
                { '<', arrows },
                { '+', arithmetic },
                { '-', arithmetic },
                { '[', brackets },
                { ']', brackets },
                { '.', dot },
                { ',', comma },
                { '(', function },
                { ')', function },
                { ':', call }
            };
        }
    }
}
