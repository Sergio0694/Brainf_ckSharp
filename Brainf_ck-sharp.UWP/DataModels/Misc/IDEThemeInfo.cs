using System.Collections.Generic;
using Windows.UI;

namespace Brainf_ck_sharp_UWP.DataModels.Misc
{
    public sealed class IDEThemeInfo
    {
        /// <summary>
        /// Gets the syntax highlight colors map for the available operators
        /// </summary>
        public IReadOnlyDictionary<char, Color> HighlightMap { get; }

        public Color Background { get; }

        public Color BreakpointsPaneBackground { get; }

        public Color CommentsColor { get; }

        public IDEThemeInfo(Color background, Color breakpoints, Color comments,
            Color arrows, Color arithmetic, Color brackets, Color dot, Color comma)
        {
            Background = background;
            BreakpointsPaneBackground = breakpoints;
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
