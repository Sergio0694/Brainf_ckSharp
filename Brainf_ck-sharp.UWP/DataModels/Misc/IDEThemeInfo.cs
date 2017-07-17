using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Brainf_ck_sharp_UWP.DataModels.Misc
{
    public sealed class IDEThemeInfo
    {
        /// <summary>
        /// Gets the syntax highlight colors map for the available operators
        /// </summary>
        public IReadOnlyDictionary<char, Color> HighlightMap { get; }

        public Color CommentsColor { get; }

        public Color Background { get; }

        public Color BreakpointsPaneBackground { get; }

        public Color LineNumberColor { get; }

        public Color BracketsGuideColor { get; }

        public int? BracketsGuideStrokesLength { get; }

        public IDEThemeInfo(Color background, Color breakpoints, Color lineNumbers,
            Color bracketsGuide, int? bracketsGuideStrokesLength,
            Color comments, Color arrows, Color arithmetic, Color brackets, Color dot, Color comma)
        {
            Background = background;
            BreakpointsPaneBackground = breakpoints;
            LineNumberColor = lineNumbers;
            BracketsGuideColor = bracketsGuide;
            BracketsGuideStrokesLength = bracketsGuideStrokesLength;
            CommentsColor = comments;
            HighlightMap = new Dictionary<char, Color>
            {
                { '>', arrows },
                { '<', arrows },
                { '+', arithmetic },
                { '-', arithmetic },
                { '[', brackets },
                { ']', brackets },
                { '.', dot },
                { ',', comma }
            };
        }
    }
}
