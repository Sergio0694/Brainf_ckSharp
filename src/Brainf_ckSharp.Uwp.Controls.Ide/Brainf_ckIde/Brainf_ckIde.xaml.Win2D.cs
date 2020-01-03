using System;
using System.Buffers;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.UI;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Helpers;
using Brainf_ckSharp.Uwp.Controls.Ide.Enums;
using Brainf_ckSharp.Uwp.Controls.Ide.Helpers;
using Brainf_ckSharp.Uwp.Controls.Ide.Models;
using Brainf_ckSharp.Uwp.Controls.Ide.Models.Abstract;
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
        /// The exact height each indentation element should have
        /// </summary>
        private const float IndentationIndicatorsTargetElementHeight = 19.9512f;

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
        /// The current array of <see cref="IndentationIndicatorBase"/> instances to render
        /// </summary>
        private IndentationIndicatorBase[] _Indicators = ArrayPool<IndentationIndicatorBase>.Shared.Rent(1);

        /// <summary>
        /// The current number of valid items in <see cref="_Indicators"/>
        /// </summary>
        private int _IndicatorsCount;

        /// <summary>
        /// Draws the IDE overlays when an update is requested
        /// </summary>
        /// <param name="sender">The sender <see cref="CanvasControl"/> instance</param>
        /// <param name="args">The <see cref="CanvasDrawEventArgs"/> for the current instance</param>
        private void IdeOverlaysCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            ref IndentationIndicatorBase r0 = ref _Indicators[0];
            int count = _IndicatorsCount;

            for (int i = 0; i < count; i++)
            {
                switch (Unsafe.Add(ref r0, i))
                {
                    case FunctionIndicator function:
                        DrawFunctionDeclaration(args.DrawingSession, GetOffsetAt(function.Y), function.Type);
                        break;
                    case BlockIndicator block:
                        DrawIndentationBlock(args.DrawingSession, GetOffsetAt(block.Y), block.Depth, block.Type, block.IsWithinFunction);
                        break;
                    case LineIndicator line:
                        DrawLine(args.DrawingSession, GetOffsetAt(line.Y), line.Type);
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
                adjustment = MathF.Floor(delta);

            return 4 + approximateOffset - adjustment;
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

        /// <summary>
        /// Updates the current indentation info to render
        /// </summary>
        /// <param name="text">The current source code being displayed</param>
        /// <param name="isSyntaxValid">Whether or not the syntax in the input source code is valid</param>
        /// <param name="numberOfLines">The current number of lines being displayed</param>
        private void UpdateIndentationInfo(string text, bool isSyntaxValid, int numberOfLines)
        {
            // Return the previous buffer
            ArrayPool<IndentationIndicatorBase>.Shared.Return(_Indicators);

            // Skip if the current syntax is not valid
            if (!isSyntaxValid)
            {
                _Indicators = ArrayPool<IndentationIndicatorBase>.Shared.Rent(1);
                _IndicatorsCount = 0;
                IdeOverlaysCanvas.Invalidate();
                return;
            }

            // Allocate the new buffer
            IndentationIndicatorBase[] indicators = _Indicators = ArrayPool<IndentationIndicatorBase>.Shared.Rent(numberOfLines);
            ref IndentationIndicatorBase indicatorsRef = ref indicators[0]; // There's always at least one line

            // Reset the pools
            Pool<LineIndicator>.Reset();
            Pool<BlockIndicator>.Reset();
            Pool<FunctionIndicator>.Reset();

            // Prepare the input text
            ReadOnlySpan<char> span = text.AsSpan();
            ref char r0 = ref MemoryMarshal.GetReference(span);
            int length = span.Length;

            // Additional tracking variables
            int
                y = 0,
                startRootDepth = 0,
                endRootDepth = 0,
                maxRootDepth = 0,
                startFunctionDepth = 0,
                endFunctionDepth = 0,
                maxFunctionDepth = 0;
            bool
                hasRootLoopStarted = false,
                hasFunctionStarted = false,
                hasFunctionEnded = false,
                hasFunctionLoopStarted = false,
                isWithinFunction = false;

            // Iterate over all the characters
            for (int i = 0; i < length; i++)
            {
                switch (Unsafe.Add(ref r0, i))
                {
                    /* For [ and ] operators, simply keep track of the current depth
                        * level both in the root and in the scope of an open function */
                    case Characters.LoopStart:
                        if (isWithinFunction)
                        {
                            endFunctionDepth++;
                            maxFunctionDepth = Math.Max(endFunctionDepth, maxFunctionDepth);
                            hasFunctionLoopStarted = true;
                        }
                        else
                        {
                            endRootDepth++;
                            maxRootDepth = Math.Max(endRootDepth, maxRootDepth);
                            hasRootLoopStarted = true;
                        }
                        break;
                    case Characters.LoopEnd:
                        if (isWithinFunction) endFunctionDepth--;
                        else endRootDepth--;
                        break;

                    /* For ( and ) operators, all is needed is to update the variables to
                        * keep track of whether or not a function declaration is currently open,
                        * and whether or not at least a function declaration was open
                        * while parsing the current line in the input source code */
                    case Characters.FunctionStart:
                        isWithinFunction = true;
                        hasFunctionStarted = true;
                        break;
                    case Characters.FunctionEnd:
                        isWithinFunction = false;
                        hasFunctionEnded = true;
                        break;

                    // Process the line info at the end of each line
                    case '\r':

                        /* If a function was declared on the current line, that takes precedence
                         * over everything else, and the function definition indicator will
                         * always be displayed in this case. Different visual modes depend
                         * on whether the function is self contained, and whether or not
                         * there is a nested self contained loop on the same line. */
                        if (hasFunctionStarted)
                        {
                            IndentationType type;
                            if (hasFunctionEnded)
                            {
                                if (isWithinFunction || endRootDepth > 0) type = IndentationType.SelfContainedAndContinuing;
                                else type = IndentationType.SelfContained;
                            }
                            else type = IndentationType.Open;

                            FunctionIndicator indicator = Pool<FunctionIndicator>.Rent();
                            indicator.Y = y;
                            indicator.Type = type;

                            Unsafe.Add(ref indicatorsRef, y) = indicator;
                        }
                        else if (hasRootLoopStarted)
                        {
                            // Indentation indicator with square UI for root loops
                            IndentationType type;
                            if (maxRootDepth > endRootDepth)
                            {
                                type = endRootDepth > 0 ? IndentationType.SelfContainedAndContinuing : IndentationType.SelfContained;
                            }
                            else type = IndentationType.Open;

                            BlockIndicator indicator = Pool<BlockIndicator>.Rent();
                            indicator.Y = y;
                            indicator.Type = type;
                            indicator.Depth = maxRootDepth;
                            indicator.IsWithinFunction = false;

                            Unsafe.Add(ref indicatorsRef, y) = indicator;
                        }
                        else if (hasFunctionLoopStarted)
                        {
                            // Indentation indicator with rounded corners for loops within functions
                            IndentationType type;
                            if (maxFunctionDepth > endFunctionDepth)
                            {
                                if (isWithinFunction || endFunctionDepth > 0) type = IndentationType.SelfContainedAndContinuing;
                                else type = IndentationType.SelfContained;
                            }
                            else type = IndentationType.Open;

                            BlockIndicator indicator = Pool<BlockIndicator>.Rent();
                            indicator.Y = y;
                            indicator.Type = type;
                            indicator.Depth = maxFunctionDepth;
                            indicator.IsWithinFunction = true;

                            Unsafe.Add(ref indicatorsRef, y) = indicator;
                        }
                        else if (hasFunctionEnded ||
                                 endRootDepth < startRootDepth ||
                                 endFunctionDepth < startFunctionDepth)
                        {
                            /* This branch is taken when the current line only contains
                             * at least a closed root or function loop. In this case, the
                             * visual mode to use depends on whether or not there is an
                             * additional external indentation scope still active at the end. */
                            IndentationType type;
                            if (isWithinFunction || endRootDepth > 0) type = IndentationType.SelfContainedAndContinuing;
                            else type = IndentationType.SelfContained;

                            LineIndicator indicator = Pool<LineIndicator>.Rent();
                            indicator.Y = y;
                            indicator.Type = type;

                            Unsafe.Add(ref indicatorsRef, y) = indicator;
                        }
                        else if (isWithinFunction || endRootDepth > 0)
                        {
                            // Active indentation level with no changes
                            LineIndicator indicator = Pool<LineIndicator>.Rent();
                            indicator.Y = y;
                            indicator.Type = IndentationType.Open;

                            Unsafe.Add(ref indicatorsRef, y) = indicator;
                        }
                        else Unsafe.Add(ref indicatorsRef, y) = null;

                        // Update the persistent trackers across lines
                        y++;
                        startRootDepth = endRootDepth;
                        maxRootDepth = endRootDepth;
                        startFunctionDepth = endFunctionDepth;
                        maxFunctionDepth = endFunctionDepth;
                        hasRootLoopStarted = false;
                        hasFunctionStarted = false;
                        hasFunctionEnded = false;
                        hasFunctionLoopStarted = false;
                        break;
                }
            }

            _Indicators = indicators;
            _IndicatorsCount = y;
            IdeOverlaysCanvas.Invalidate();
        }
    }
}
