using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Git;
using Brainf_ckSharp.Git.Enums;
using Brainf_ckSharp.Uwp.Controls.Ide.Enums;
using Brainf_ckSharp.Uwp.Controls.Ide.Models;
using Brainf_ckSharp.Uwp.Controls.Ide.Models.Abstract;
using Microsoft.Toolkit.HighPerformance.Buffers;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    public sealed partial class Brainf_ckIde
    {
        /// <summary>
        /// Updates the current line diff indicators when the displayed text changes
        /// </summary>
        /// <param name="text">The current source code being displayed</param>
        private void UpdateDiffInfo(string text)
        {
            MemoryOwner<LineModificationType> diff = LineDiffer.ComputeDiff(_LoadedText, text, Characters.CarriageReturn);

            ref LineModificationType oldRef = ref _DiffIndicators.DangerousGetReference();
            ref LineModificationType newRef = ref diff.DangerousGetReference();
            int length = Math.Min(_DiffIndicators.Length, diff.Length);

            // Maintain the saved indicators
            for (int i = 0; i < length; i++)
                if (Unsafe.Add(ref oldRef, i) == LineModificationType.Saved &&
                    Unsafe.Add(ref newRef, i) == LineModificationType.None)
                    Unsafe.Add(ref newRef, i) = LineModificationType.Saved;

            _DiffIndicators.Dispose();

            // The edit box always ends with a final \r that can't be removed by the user.
            // Slicing out the last item prevents the modified marker from being displayed
            // below the last line actually being typed by the user.
            _DiffIndicators = diff.Slice(0, diff.Length - 1);
        }

        /// <summary>
        /// Updates the current line diff when the current text is marked as saved
        /// </summary>
        private void UpdateDiffInfo()
        {
            int size = _DiffIndicators.Length;

            if (size == 0) return;

            ref LineModificationType r0 = ref _DiffIndicators.DangerousGetReference();

            for (int i = 0; i < size; i++)
                if (Unsafe.Add(ref r0, i) == LineModificationType.Modified)
                    Unsafe.Add(ref r0, i) = LineModificationType.Saved;
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
    }
}
