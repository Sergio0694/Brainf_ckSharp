using Windows.UI;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Toolkit.Uwp.Helpers;

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    public sealed partial class Brainf_ckIde
    {
        /// <summary>
        /// The middle padding for the start of the git diff markers
        /// </summary>
        private const int GitDiffMarkersMiddleMargin = 53;

        /// <summary>
        /// The stroke to use to draw the center of git diff markers
        /// </summary>
        private const float GitDiffMarkerStrokeWidth = 4;

        /// <summary>
        /// The stroke to use to draw the outline of git diff markers
        /// </summary>
        private const float GitDiffMarkerOutlineWidth = 6;

        /// <summary>
        /// The color used to draw the stroke of git diff markers for a saved line
        /// </summary>
        private static readonly Color SavedGitDiffMarkerStrokeColor = "#FF577430".ToColor();

        /// <summary>
        /// The color used to draw the outline of git diff markers for a saved line
        /// </summary>
        private static readonly Color SavedGitDiffMarkerOutlineColor = "#FF3A4927".ToColor();

        /// <summary>
        /// The color used to draw the stroke of git diff markers for a modified line
        /// </summary>
        private static readonly Color ModifiedGitDiffMarkerStrokeColor = "#FFEFF284".ToColor();

        /// <summary>
        /// The color used to draw the outline of git diff markers for a modified line
        /// </summary>
        private static readonly Color ModifiedGitDiffMarkerOutlineColor = "#FF868851".ToColor();

        /// <summary>
        /// The left padding for the start of the indentation indicators
        /// </summary>
        private const int IndentationIndicatorsLeftMargin = 59;

        /// <summary>
        /// The middle left padding for the start of vertical lines
        /// </summary>
        private const int IndentationIndicatorsMiddleMargin = IndentationIndicatorsLeftMargin + IndentationIndicatorBlockSize / 2;

        /// <summary>
        /// The right edge for horizontal lines
        /// </summary>
        private const int IndentationIndicatorsRightMargin = IndentationIndicatorsLeftMargin + IndentationIndicatorBlockSize;

        /// <summary>
        /// The size of geometrical indentation indicator blocks
        /// </summary>
        private const int IndentationIndicatorBlockSize = 10;

        /// <summary>
        /// The height of each indentation element
        /// </summary>
        private const int IndentationIndicatorsElementHeight = 20;

        /// <summary>
        /// The exact height each indentation element should have
        /// </summary>
        private const float IndentationIndicatorsTargetElementHeight = 19.9512f;

        /// <summary>
        /// The vertical margin for the indentation guides
        /// </summary>
        private const float IndentationIndicatorsVerticalOffsetMargin = 4;

        /// <summary>
        /// The color used to draw outlines for the indentation indicators
        /// </summary>
        private static readonly Color OutlineColor = "#FFA5A5A5".ToColor();

        /// <summary>
        /// The text for the background of the function declaration indicator
        /// </summary>
        private static readonly Color FunctionBackgroundColor = "#FF3F3F3F".ToColor();

        /// <summary>
        /// The colors used to render the counters for the indentation depth
        /// </summary>
        private static readonly Color TextColor = "#FFE2E2E2".ToColor();

        /// <summary>
        /// The text format for text overlays
        /// </summary>
        private static readonly CanvasTextFormat TextFormat = new CanvasTextFormat { FontSize = 8 };

        /// <summary>
        /// The top padding for the start of the breakpoint indicators
        /// </summary>
        private const int BreakpointIndicatorTopMargin = 2;

        /// <summary>
        /// The left padding for the start of the breakpoint indicators
        /// </summary>
        private const int BreakpointIndicatorLeftMargin = 2;

        /// <summary>
        /// The height of each breakpoint indicator
        /// </summary>
        private const int BreakpointIndicatorElementSize = 16;

        /// <summary>
        /// The color used to draw the outline of a breakpoint indicator
        /// </summary>
        private static readonly Color BreakpointIndicatorBorderColor = "#FFF4DFDD".ToColor();

        /// <summary>
        /// The color used to fill the breakpoint indicators
        /// </summary>
        private static readonly Color BreakpointIndicatorFillColor = "#FFE51400".ToColor();

        /// <summary>
        /// The radius of breakpoint areas
        /// </summary>
        private const int BreakpointAreaCornerRadius = 2;

        /// <summary>
        /// The color used for the borders of breakpoint areas
        /// </summary>
        private static readonly Color BreakpointAreaBorderColor = Colors.DimGray;

        /// <summary>
        /// The color used to fill breakpoint areas
        /// </summary>
        private static readonly Color BreakpointAreaFillColor = "#FF91272C".ToColor();
    }
}
