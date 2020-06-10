using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.UI.Text;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Uwp.Controls.Ide.Enums;
using Brainf_ckSharp.Uwp.Controls.Ide.Models;
using Brainf_ckSharp.Uwp.Controls.Ide.Models.Abstract;
using Microsoft.Toolkit.HighPerformance.Buffers;
using Microsoft.Toolkit.HighPerformance.Extensions;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    public sealed partial class Brainf_ckIde
    {
        /// <summary>
        /// Updates the current indentation info to render
        /// </summary>
        /// <param name="text">The current source code being displayed</param>
        /// <param name="isSyntaxValid">Whether or not the syntax in the input source code is valid</param>
        /// <param name="numberOfLines">The current number of lines being displayed</param>
        private void UpdateIndentationInfo(string text, bool isSyntaxValid, int numberOfLines)
        {
            // Return the previous buffer
            _IndentationIndicators.Dispose();

            // Skip if the current syntax is not valid
            if (!isSyntaxValid)
            {
                _IndentationIndicators = MemoryOwner<IndentationIndicatorBase?>.Empty;

                return;
            }

            // Allocate the new buffer
            MemoryOwner<IndentationIndicatorBase?> indicators = _IndentationIndicators = MemoryOwner<IndentationIndicatorBase?>.Allocate(numberOfLines);
            ref IndentationIndicatorBase? indicatorsRef = ref indicators.DangerousGetReference(); // There's always at least one line

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
                    // For [ and ] operators, simply keep track of the current depth
                    // level both in the root and in the scope of an open function
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

                    // For ( and ) operators, all is needed is to update the variables to
                    // keep track of whether or not a function declaration is currently open,
                    // and whether or not at least a function declaration was open
                    // while parsing the current line in the input source code
                    case Characters.FunctionStart:
                        isWithinFunction = true;
                        hasFunctionStarted = true;
                        break;
                    case Characters.FunctionEnd:
                        isWithinFunction = false;
                        hasFunctionEnded = true;
                        break;

                    // Process the line info at the end of each line
                    case Characters.CarriageReturn:

                        // If a function was declared on the current line, that takes precedence
                        // over everything else, and the function definition indicator will
                        // always be displayed in this case. Different visual modes depend
                        // on whether the function is self contained, and whether or not
                        // there is a nested self contained loop on the same line.
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
                            indicator.Type = type;
                            indicator.Depth = maxFunctionDepth;
                            indicator.IsWithinFunction = true;

                            Unsafe.Add(ref indicatorsRef, y) = indicator;
                        }
                        else if (hasFunctionEnded ||
                                 endRootDepth < startRootDepth ||
                                 endFunctionDepth < startFunctionDepth)
                        {
                            // This branch is taken when the current line only contains
                            // at least a closed root or function loop. In this case, the
                            // visual mode to use depends on whether or not there is an
                            // additional external indentation scope still active at the end.
                            IndentationType type;
                            if (isWithinFunction || endRootDepth > 0) type = IndentationType.SelfContainedAndContinuing;
                            else type = IndentationType.SelfContained;

                            LineIndicator indicator = Pool<LineIndicator>.Rent();
                            indicator.Type = type;

                            Unsafe.Add(ref indicatorsRef, y) = indicator;
                        }
                        else if (isWithinFunction || endRootDepth > 0)
                        {
                            // Active indentation level with no changes
                            LineIndicator indicator = Pool<LineIndicator>.Rent();
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
        }

        /// <summary>
        /// Updates the info for the breakpoints to display
        /// </summary>
        private void UpdateBreakpointsInfo()
        {
            int totalBreakpoints = BreakpointIndicators.Count;

            // If there are no breakpoints, do nothing
            if (totalBreakpoints == 0)
            {
                if (_BreakpointAreas.Length != 0)
                {
                    _BreakpointAreas.Dispose();

                    _BreakpointAreas = MemoryOwner<Rect>.Empty;
                }

                return;
            }

            // Rent a buffer to track the current line numbers
            using MemoryOwner<int> lineNumbers = MemoryOwner<int>.Allocate(totalBreakpoints);
            ref int lineNumbersRef = ref lineNumbers.DangerousGetReference();
            int currentLineNumber = 0;

            // Copy the current line numbers to a buffer
            foreach (var entry in BreakpointIndicators)
            {
                Unsafe.Add(ref lineNumbersRef, currentLineNumber++) = entry.Key;
            }

            // Get the underlying array (no Span<T>.Sort API available on UWP)
            _ = MemoryMarshal.TryGetArray(lineNumbers.Memory, out ArraySegment<int> segment);

            // Sort the current line numbers (they might have been added not sequentially)
            Array.Sort(segment.Array, 0, totalBreakpoints);

            // Create an oversized buffer for the computer areas, and tracking variables
            MemoryOwner<Rect> breakpointAreas = MemoryOwner<Rect>.Allocate(totalBreakpoints);
            ref Rect breakpointAreasRef = ref breakpointAreas.DangerousGetReference();
            int
                currentBreakpointIndex = 0,
                currentTargetLineNumber = Unsafe.Add(ref lineNumbersRef, 0),
                currentTextIndex = 0,
                validatedBreakpoints = 0;
            currentLineNumber = 1; // The line count starts at 1

            // Go through all the available lines of text
            foreach (var line in Text.Tokenize(Characters.CarriageReturn))
            {
                if (currentTargetLineNumber == currentLineNumber)
                {
                    int
                        firstOperatorOffset = -1,
                        lastOperatorOffset = 0;

                    // If the current line is marked as containing a breakpoint,
                    // validate it to make sure there is still at least one operator
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (Brainf_ckParser.IsOperator(line[i]))
                        {
                            firstOperatorOffset = lastOperatorOffset = i;

                            for (int j = i + 1; j < line.Length; j++)
                            {
                                if (Brainf_ckParser.IsOperator(line[j])) lastOperatorOffset = j;
                                else goto ProcessLineAnalysisResults;
                            }

                            goto ProcessLineAnalysisResults;
                        }
                    }

                    ProcessLineAnalysisResults:

                    // If there are no operators left, remove the breakpoint
                    if (firstOperatorOffset == -1)
                    {
                        BreakpointIndicators.Remove(currentTargetLineNumber);

                        totalBreakpoints--;
                    }
                    else
                    {
                        // Get the text range for the first operators interval
                        ITextRange range = CodeEditBox.Document.GetRange(
                            currentTextIndex + firstOperatorOffset,
                            currentTextIndex + lastOperatorOffset + 1);

                        // Extract the target coordinates for the text range
                        range.GetRect(
                            PointOptions.Transform,
                            out Unsafe.Add(ref breakpointAreasRef, validatedBreakpoints++),
                            out _);
                    }

                    if (validatedBreakpoints == totalBreakpoints) break;
                    if (currentBreakpointIndex++ == totalBreakpoints) break;

                    // Update the target breakpoint line number
                    currentTargetLineNumber = Unsafe.Add(ref lineNumbersRef, currentBreakpointIndex);
                }

                currentLineNumber++;
                currentTextIndex += line.Length + 1;
            }

            _BreakpointAreas.Dispose();

            _BreakpointAreas = breakpointAreas.Slice(0, validatedBreakpoints);
        }
    }
}
