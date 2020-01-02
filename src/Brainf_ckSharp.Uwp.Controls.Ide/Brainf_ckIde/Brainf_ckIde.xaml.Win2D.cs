using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Windows.UI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Toolkit.Uwp.Helpers;

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    public sealed partial class Brainf_ckIde
    {
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
        /// Draws the IDE overlays when an update is requested
        /// </summary>
        /// <param name="sender">The sender <see cref="CanvasControl"/> instance</param>
        /// <param name="args">The <see cref="CanvasDrawEventArgs"/> for the current instance</param>
        private void IdeOverlaysCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            // TODO
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float GetOffsetAt(int i) => 4 + IndentationIndicatorsElementHeight * i;

        /// <summary>
        /// Draws a single vertical indentation line
        /// </summary>
        /// <param name="session">The target <see cref="CanvasDrawingSession"/> instance</param>
        /// <param name="offset">The current vertical offset to start drawing the line from</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DrawLine(CanvasDrawingSession session, float offset)
        {
            session.DrawLine(IndentationIndicatorsMiddleMargin, offset, IndentationIndicatorsMiddleMargin, offset + IndentationIndicatorsElementHeight, OutlineColor);
        }

        /// <summary>
        /// Draws an indentation block
        /// </summary>
        /// <param name="session">The target <see cref="CanvasDrawingSession"/> instance</param>
        /// <param name="depth">The current indentation block depth</param>
        /// <param name="offset">The current vertical offset to start drawing the indentation block from</param>
        /// <param name="isSelfContained">Whether or not the indentation block is self contained</param>
        /// <param name="isWithinFunction">Whether or not the current block is contained within a function</param>
        private static void DrawIndentationBlock(CanvasDrawingSession session, float offset, int depth, bool isSelfContained, bool isWithinFunction)
        {
            if (isWithinFunction)
            {
                session.DrawRoundedRectangle(IndentationIndicatorsLeftMargin, offset, IndentationIndicatorBlockSize, IndentationIndicatorBlockSize, 2, 2, OutlineColor);
            }
            else session.DrawRectangle(IndentationIndicatorsLeftMargin, offset, IndentationIndicatorBlockSize, IndentationIndicatorBlockSize, OutlineColor);

            string text = depth <= 9 ? depth.ToString() : "-";
            session.DrawText(text, IndentationIndicatorsLeftMargin + 3, offset - 1, TextColor, TextFormat);

            /* If the function has a depth greater than 1, or is within a function or is not self
             * contained, it means that its vertical guide must reach the end of the current slot.
             * Otherwise, the vertical guide must only reach half of the remaining slot space,
             * and be immediately connected to the horizontal guide to indicate the closed level. */
            if (depth > 1 || isWithinFunction || !isSelfContained)
            {
                session.DrawLine(IndentationIndicatorsMiddleMargin, offset + IndentationIndicatorBlockSize, IndentationIndicatorsMiddleMargin, offset + IndentationIndicatorsElementHeight, OutlineColor);
            }
            else
            {
                float middleOffset = offset + (IndentationIndicatorsElementHeight + IndentationIndicatorBlockSize) / 2f;
                session.DrawLine(IndentationIndicatorsMiddleMargin, offset + IndentationIndicatorBlockSize, IndentationIndicatorsMiddleMargin, middleOffset, OutlineColor);
            }

            if (isSelfContained)
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
        /// <param name="isSelfContained">Whether or not the function declaration is self contained</param>
        private static void DrawFunctionDeclaration(CanvasDrawingSession session, float offset, bool isSelfContained)
        {
            session.FillRoundedRectangle(IndentationIndicatorsLeftMargin, offset, IndentationIndicatorBlockSize, IndentationIndicatorBlockSize, 9999, 9999, FunctionBackgroundColor);
            session.DrawRoundedRectangle(IndentationIndicatorsLeftMargin, offset, IndentationIndicatorBlockSize, IndentationIndicatorBlockSize, 9999, 9999, OutlineColor);
            session.DrawText("f", IndentationIndicatorsLeftMargin + 4f, offset, TextColor, TextFormat);

            if (isSelfContained)
            {
                float
                    horizontalOffset = IndentationIndicatorsMiddleMargin - 0.5f,
                    middleOffset = offset + (IndentationIndicatorsElementHeight + IndentationIndicatorBlockSize) / 2f;
                session.DrawLine(IndentationIndicatorsMiddleMargin, offset + IndentationIndicatorBlockSize, IndentationIndicatorsMiddleMargin, middleOffset, OutlineColor);
                session.DrawLine(horizontalOffset, middleOffset, IndentationIndicatorsRightMargin, middleOffset, OutlineColor);
            }
            else session.DrawLine(IndentationIndicatorsMiddleMargin, offset + IndentationIndicatorBlockSize, IndentationIndicatorsMiddleMargin, offset + IndentationIndicatorsElementHeight, OutlineColor);
        }
    }
}
