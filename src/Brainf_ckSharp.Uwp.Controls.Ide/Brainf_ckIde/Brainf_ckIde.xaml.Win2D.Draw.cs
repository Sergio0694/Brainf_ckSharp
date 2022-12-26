using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.UI;
using Brainf_ckSharp.Git.Enums;
using Brainf_ckSharp.Uwp.Controls.Ide.Enums;
using Brainf_ckSharp.Uwp.Controls.Ide.Models;
using Brainf_ckSharp.Uwp.Controls.Ide.Models.Abstract;
using Microsoft.Collections.Extensions;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using CommunityToolkit.HighPerformance.Buffers;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide;

public sealed partial class Brainf_ckIde
{
    /// <summary>
    /// The map of breakpoints in use
    /// </summary>
    private readonly DictionarySlim<int, float> BreakpointIndicators = new();

    /// <summary>
    /// The precomputed areas of breakpoints to display
    /// </summary>
    private MemoryOwner<Rect> _BreakpointAreas = MemoryOwner<Rect>.Empty;

    /// <summary>
    /// The current buffer of line diff indicators
    /// </summary>
    /// <remarks>The initial size is 1 since it corresponds to the default '\r' character in the control</remarks>
    private MemoryOwner<LineModificationType> _DiffIndicators = MemoryOwner<LineModificationType>.Allocate(1);

    /// <summary>
    /// The current array of <see cref="IndentationIndicatorBase"/> instances to render
    /// </summary>
    private MemoryOwner<IndentationIndicatorBase?> _IndentationIndicators = MemoryOwner<IndentationIndicatorBase?>.Empty;

    /// <summary>
    /// Draws the IDE overlays when an update is requested
    /// </summary>
    /// <param name="sender">The sender <see cref="CanvasControl"/> instance</param>
    /// <param name="args">The <see cref="CanvasDrawEventArgs"/> for the current instance</param>
    private void IdeOverlaysCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        // Git diff indicators
        int i = 0;
        foreach (var modification in _DiffIndicators.Span)
        {
            switch (modification)
            {
                case LineModificationType.Modified:
                    DrawDiffMarker(args.DrawingSession, GetOffsetAt(i), ModifiedGitDiffMarkerStrokeColor, ModifiedGitDiffMarkerOutlineColor);
                    break;
                case LineModificationType.Saved:
                    DrawDiffMarker(args.DrawingSession, GetOffsetAt(i), SavedGitDiffMarkerStrokeColor, SavedGitDiffMarkerOutlineColor);
                    break;
            }

            i++;
        }

        // Indentation indicators
        i = 0;
        foreach (var indicator in _IndentationIndicators.Span)
        {
            if (indicator is not null)
            {
                // Manually perform the type checks and unsafe casts to skip
                // the multiple null and type checking produced by the C# compiler
                if (indicator.GetType() == typeof(FunctionIndicator))
                {
                    FunctionIndicator function = Unsafe.As<FunctionIndicator>(indicator)!;

                    DrawFunctionDeclaration(args.DrawingSession, GetOffsetAt(i) + IndentationIndicatorsVerticalOffsetMargin, function.Type);
                }
                else if (indicator.GetType() == typeof(BlockIndicator))
                {
                    BlockIndicator block = Unsafe.As<BlockIndicator>(indicator)!;

                    DrawIndentationBlock(args.DrawingSession, GetOffsetAt(i) + IndentationIndicatorsVerticalOffsetMargin, block.Depth, block.Type, block.IsWithinFunction);
                }
                else if (indicator.GetType() == typeof(LineIndicator))
                {
                    LineIndicator line = Unsafe.As<LineIndicator>(indicator)!;

                    DrawLine(args.DrawingSession, GetOffsetAt(i) + IndentationIndicatorsVerticalOffsetMargin, line.Type);
                }
            }

            i++;
        }

        // Breakpoints markers
        foreach (var pair in BreakpointIndicators)
        {
            DrawBreakpointIndicator(args.DrawingSession, pair.Value);
        }
    }

    /// <summary>
    /// Draws the text overlays when an update is requested
    /// </summary>
    /// <param name="sender">The sender <see cref="CanvasControl"/> instance</param>
    /// <param name="args">The <see cref="CanvasDrawEventArgs"/> for the current instance</param>
    private void CodeEditBox_OnDrawOverlays(CanvasControl sender, CanvasDrawEventArgs args)
    {
        // Breakpoints areas
        foreach (var rect in _BreakpointAreas.Span)
        {
            DrawBreakpointArea(args.DrawingSession, rect);
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
        Debug.Assert(offset >= 0);
        Debug.Assert(depth >= 1);

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

    /// <summary>
    /// Draws a breakpoint indicator
    /// </summary>
    /// <param name="session">The target <see cref="CanvasDrawingSession"/> instance</param>
    /// <param name="offset">The current vertical offset to start drawing the breakpoint indicator from</param>
    private static void DrawBreakpointIndicator(CanvasDrawingSession session, float offset)
    {
        offset += BreakpointIndicatorTopMargin;

        session.FillRoundedRectangle(BreakpointIndicatorLeftMargin, offset, BreakpointIndicatorElementSize, BreakpointIndicatorElementSize, 9999, 9999, BreakpointIndicatorFillColor);
        session.DrawRoundedRectangle(BreakpointIndicatorLeftMargin, offset, BreakpointIndicatorElementSize, BreakpointIndicatorElementSize, 9999, 9999, BreakpointIndicatorBorderColor);
    }

    /// <summary>
    /// Draws a breakpoint area
    /// </summary>
    /// <param name="session">The target <see cref="CanvasDrawingSession"/> instance</param>
    /// <param name="rect">The target area to draw over</param>
    private static void DrawBreakpointArea(CanvasDrawingSession session, Rect rect)
    {
        float
            x = (float)rect.Left + 8,
            y = (float)rect.Top + 78,
            width = (float)rect.Width + 2,
            height = (float)rect.Height;

        session.FillRoundedRectangle(x, y, width, height, BreakpointAreaCornerRadius, BreakpointAreaCornerRadius, BreakpointAreaBorderColor);
        session.FillRoundedRectangle(x + 1, y + 1, width - 2, height - 2, BreakpointAreaCornerRadius, BreakpointAreaCornerRadius, BreakpointAreaFillColor);
    }
}
