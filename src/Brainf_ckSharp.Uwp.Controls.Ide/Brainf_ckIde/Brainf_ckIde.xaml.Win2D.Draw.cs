using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Windows.UI;
using Brainf_ckSharp.Git.Enums;
using Brainf_ckSharp.Uwp.Controls.Ide.Enums;
using Brainf_ckSharp.Uwp.Controls.Ide.Models;
using Brainf_ckSharp.Uwp.Controls.Ide.Models.Abstract;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    public sealed partial class Brainf_ckIde
    {
        /// <summary>
        /// The current buffer of line diff indicators
        /// </summary>
        private MemoryOwner<LineModificationType> _DiffIndicators = MemoryOwner<LineModificationType>.Allocate(0);

        /// <summary>
        /// The current array of <see cref="IndentationIndicatorBase"/> instances to render
        /// </summary>
        private IndentationIndicatorBase[] _IndentationIndicators = ArrayPool<IndentationIndicatorBase>.Shared.Rent(1);

        /// <summary>
        /// The current number of valid items in <see cref="_IndentationIndicators"/>
        /// </summary>
        private int _IndentationIndicatorsCount;

        /// <summary>
        /// Draws the IDE overlays when an update is requested
        /// </summary>
        /// <param name="sender">The sender <see cref="CanvasControl"/> instance</param>
        /// <param name="args">The <see cref="CanvasDrawEventArgs"/> for the current instance</param>
        private void IdeOverlaysCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            int diffIndicatorsCount = _DiffIndicators.Size;

            if (diffIndicatorsCount > 0)
            {
                ref LineModificationType diffRef = ref _DiffIndicators.GetReference();

                for (int i = 0; i < diffIndicatorsCount; i++)
                {
                    switch (Unsafe.Add(ref diffRef, i))
                    {
                        case LineModificationType.Modified:
                            DrawDiffMarker(args.DrawingSession, GetOffsetAt(i), ModifiedGitDiffMarkerStrokeColor, ModifiedGitDiffMarkerOutlineColor);
                            break;
                        case LineModificationType.Saved:
                            DrawDiffMarker(args.DrawingSession, GetOffsetAt(i), SavedGitDiffMarkerStrokeColor, SavedGitDiffMarkerOutlineColor);
                            break;
                    }
                }
            }

            ref IndentationIndicatorBase indentationIndicatorsRef = ref _IndentationIndicators[0];
            int indentationIndicatorsCount = _IndentationIndicatorsCount;

            for (int i = 0; i < indentationIndicatorsCount; i++)
            {
                switch (Unsafe.Add(ref indentationIndicatorsRef, i))
                {
                    case FunctionIndicator function:
                        DrawFunctionDeclaration(args.DrawingSession, GetOffsetAt(function.Y) + IndentationIndicatorsVerticalOffsetMargin, function.Type);
                        break;
                    case BlockIndicator block:
                        DrawIndentationBlock(args.DrawingSession, GetOffsetAt(block.Y) + IndentationIndicatorsVerticalOffsetMargin, block.Depth, block.Type, block.IsWithinFunction);
                        break;
                    case LineIndicator line:
                        DrawLine(args.DrawingSession, GetOffsetAt(line.Y) + IndentationIndicatorsVerticalOffsetMargin, line.Type);
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the vertical offset corresponding to a given line
        /// </summary>
        /// <param name="i">The line for which to retrieve the vertical offset</param>
        /// <returns>The vertical offset for a line at position <param name="i"></param></returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float GetOffsetAt(int i)
        {
            float
                approximateOffset = IndentationIndicatorsElementHeight * i,
                targetOffset = IndentationIndicatorsTargetElementHeight * i,
                delta = approximateOffset - targetOffset,
                adjustment = MathF.Floor(delta),
                target = approximateOffset - adjustment;

            return target;
        }

        /// <summary>
        /// Draws a git diff marker at a specified offset
        /// </summary>
        /// <param name="session">The target <see cref="CanvasDrawingSession"/> instance</param>
        /// <param name="offset">The current vertical offset to start drawing the marker from</param>
        /// <param name="strokeColor">The color to use for the marker stroke</param>
        /// <param name="outlineColor">The color to use for the marker outline</param>
        private static void DrawDiffMarker(
            CanvasDrawingSession session,
            float offset,
            Color strokeColor,
            Color outlineColor)
        {
            session.DrawLine(GitDiffMarkersMiddleMargin, offset, GitDiffMarkersMiddleMargin, offset + IndentationIndicatorsElementHeight, outlineColor, GitDiffMarkerOutlineWidth);
            session.DrawLine(GitDiffMarkersMiddleMargin, offset, GitDiffMarkersMiddleMargin, offset + IndentationIndicatorsElementHeight, strokeColor, GitDiffMarkerStrokeWidth);
        }

        /// <summary>
        /// Draws a single vertical indentation line
        /// </summary>
        /// <param name="session">The target <see cref="CanvasDrawingSession"/> instance</param>
        /// <param name="offset">The current vertical offset to start drawing the line from</param>
        /// <param name="indentationType">The type of line to draw</param>
        private static void DrawLine(
            CanvasDrawingSession session,
            float offset,
            IndentationType indentationType)
        {
            // Vertical guide
            if ((indentationType & IndentationType.FullSize) != 0)
            {
                session.DrawLine(IndentationIndicatorsMiddleMargin, offset, IndentationIndicatorsMiddleMargin, offset + IndentationIndicatorsElementHeight, OutlineColor);
            }
            else
            {
                float middleOffset = offset + (IndentationIndicatorsElementHeight + IndentationIndicatorBlockSize) / 2f;
                session.DrawLine(IndentationIndicatorsMiddleMargin, offset, IndentationIndicatorsMiddleMargin, middleOffset, OutlineColor);
            }

            // Horizontal marker
            if ((indentationType & IndentationType.IsClosing) != 0)
            {
                float
                    horizontalOffset = IndentationIndicatorsMiddleMargin - 0.5f,
                    middleOffset = offset + (IndentationIndicatorsElementHeight + IndentationIndicatorBlockSize) / 2f + 0.5f;
                session.DrawLine(horizontalOffset, middleOffset, IndentationIndicatorsRightMargin, middleOffset, OutlineColor);
            }
        }

        /// <summary>
        /// Draws an indentation block
        /// </summary>
        /// <param name="session">The target <see cref="CanvasDrawingSession"/> instance</param>
        /// <param name="depth">The current indentation block depth</param>
        /// <param name="offset">The current vertical offset to start drawing the indentation block from</param>
        /// <param name="indentationType">The type of indentation block to draw</param>
        /// <param name="isWithinFunction">Whether or not the current block is contained within a function</param>
        private static void DrawIndentationBlock(
            CanvasDrawingSession session,
            float offset,
            int depth,
            IndentationType indentationType,
            bool isWithinFunction)
        {
            DebugGuard.MustBeGreaterThanOrEqualTo(offset, 0, nameof(offset));
            DebugGuard.MustBeGreaterThanOrEqualTo(depth, 1, nameof(depth));

            // Indentation block
            if (isWithinFunction)
            {
                session.DrawRoundedRectangle(IndentationIndicatorsLeftMargin, offset, IndentationIndicatorBlockSize, IndentationIndicatorBlockSize, 2, 2, OutlineColor);
            }
            else session.DrawRectangle(IndentationIndicatorsLeftMargin, offset, IndentationIndicatorBlockSize, IndentationIndicatorBlockSize, OutlineColor);

            // Depth level counter
            string text = depth <= 9 ? depth.ToString() : "•";
            session.DrawText(text, IndentationIndicatorsLeftMargin + 3, offset - 1, TextColor, TextFormat);

            // Vertical guide
            if ((indentationType & IndentationType.FullSize) != 0)
            {
                session.DrawLine(IndentationIndicatorsMiddleMargin, offset + IndentationIndicatorBlockSize, IndentationIndicatorsMiddleMargin, offset + IndentationIndicatorsElementHeight, OutlineColor);
            }
            else
            {
                float middleOffset = offset + (IndentationIndicatorsElementHeight + IndentationIndicatorBlockSize) / 2f;
                session.DrawLine(IndentationIndicatorsMiddleMargin, offset + IndentationIndicatorBlockSize, IndentationIndicatorsMiddleMargin, middleOffset, OutlineColor);
            }

            // Horizontal marker
            if ((indentationType & IndentationType.IsClosing) != 0)
            {
                float
                    horizontalOffset = IndentationIndicatorsMiddleMargin - 0.5f,
                    middleOffset = offset + (IndentationIndicatorsElementHeight + IndentationIndicatorBlockSize) / 2f + 0.5f;
                session.DrawLine(horizontalOffset, middleOffset, IndentationIndicatorsRightMargin, middleOffset, OutlineColor);
            }
        }

        /// <summary>
        /// Draws a function declaration
        /// </summary>
        /// <param name="session">The target <see cref="CanvasDrawingSession"/> instance</param>
        /// <param name="offset">The current vertical offset to start drawing the function declaration from</param>
        /// <param name="indentationType">The type of line to draw</param>
        private static void DrawFunctionDeclaration(
            CanvasDrawingSession session,
            float offset,
            IndentationType indentationType)
        {
            session.FillRoundedRectangle(IndentationIndicatorsLeftMargin, offset, IndentationIndicatorBlockSize, IndentationIndicatorBlockSize, 9999, 9999, FunctionBackgroundColor);
            session.DrawRoundedRectangle(IndentationIndicatorsLeftMargin, offset, IndentationIndicatorBlockSize, IndentationIndicatorBlockSize, 9999, 9999, OutlineColor);
            session.DrawText("f", IndentationIndicatorsLeftMargin + 4f, offset, TextColor, TextFormat);

            // Vertical guide
            if ((indentationType & IndentationType.FullSize) != 0)
            {
                session.DrawLine(IndentationIndicatorsMiddleMargin, offset + IndentationIndicatorBlockSize, IndentationIndicatorsMiddleMargin, offset + IndentationIndicatorsElementHeight, OutlineColor);
            }
            else
            {
                float middleOffset = offset + (IndentationIndicatorsElementHeight + IndentationIndicatorBlockSize) / 2f;
                session.DrawLine(IndentationIndicatorsMiddleMargin, offset + IndentationIndicatorBlockSize, IndentationIndicatorsMiddleMargin, middleOffset, OutlineColor);
            }

            // Horizontal marker
            if ((indentationType & IndentationType.IsClosing) != 0)
            {
                float
                    horizontalOffset = IndentationIndicatorsMiddleMargin - 0.5f,
                    middleOffset = offset + (IndentationIndicatorsElementHeight + IndentationIndicatorBlockSize) / 2f + 0.5f;
                session.DrawLine(horizontalOffset, middleOffset, IndentationIndicatorsRightMargin, middleOffset, OutlineColor);
            }
        }
    }
}
