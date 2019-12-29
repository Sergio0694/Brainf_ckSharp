using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Brainf_ckSharp.Buffers;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Helpers;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    public sealed partial class Brainf_ckEditBox
    {
        /// <summary>
        /// The current sequence of brackets being displayed in the text
        /// </summary>
        private MemoryOwner<(int Start, int End)> _BracketsList = MemoryOwner<(int, int)>.Allocate(0);

        /// <summary>
        /// Tries to update the current sequence of brackets displayed in the text
        /// </summary>
        /// <returns><see langword="true"/> if the sequence was updated, <see langword="false"/> otherwise</returns>
        [Pure]
        private bool TryUpdateBracketsList()
        {
            DebugGuard.MustBeTrue(_IsSyntaxValid, nameof(_IsSyntaxValid));

            // Prepare the current text
            ReadOnlySpan<char> text = _Text.AsSpan();
            ref char r0 = ref MemoryMarshal.GetReference(text);
            int length = text.Length;

            // Temporary buffers, just in the original method in the core library
            int tempBuffersLength = length / 2 + 1;
            using UnsafeSpan<(int, int)> rootTempIndices = UnsafeSpan<(int, int)>.Allocate(tempBuffersLength);
            using UnsafeSpan<(int, int)> functionTempIndices = UnsafeSpan<(int, int)>.Allocate(tempBuffersLength);
            ref (int Index, int Y) rootTempIndicesRef = ref rootTempIndices.GetReference();
            ref (int Index, int Y) functionTempIndicesRef = ref functionTempIndices.GetReference();
            int
                jumps = 0,
                y = 0;

            // Target buffer
            MemoryOwner<(int, int)> jumpTable = MemoryOwner<(int, int)>.Allocate(length);
            ref (int, int) jumpTableRef = ref jumpTable.GetReference();

            // Go through the executable to build the jump table for each open parenthesis or square bracket
            for (int r = 0, f = -1, i = 0; i < length; i++)
            {
                switch (Unsafe.Add(ref r0, i))
                {
                    // Track each loop start
                    case Characters.LoopStart:
                        if (f == -1) Unsafe.Add(ref rootTempIndicesRef, r++) = (i, y);
                        else Unsafe.Add(ref functionTempIndicesRef, f++) = (i, y);
                        break;

                    // Track loop ends if there is at least one line of difference
                    case Characters.LoopEnd:
                        var start = f == -1
                            ? Unsafe.Add(ref rootTempIndicesRef, --r)
                            : Unsafe.Add(ref functionTempIndicesRef, --f);
                        if (start.Y < y)
                        {
                            Unsafe.Add(ref jumpTableRef, jumps++) = (start.Index, i);
                        }
                        break;

                    // Track functions, starting at 1 so that index 0 stores the function brackets
                    case Characters.FunctionStart:
                        f = 1;
                        functionTempIndicesRef = (i, y);
                        break;
                    case Characters.FunctionEnd:
                        f = -1;
                        if (functionTempIndicesRef.Y < y)
                        {
                            Unsafe.Add(ref jumpTableRef, jumps++) = (functionTempIndicesRef.Index, i);
                        }
                        break;

                    // Track each new line
                    case '\r':
                        y++;
                        break;
                }
            }

            // Skip the update if both sequences match
            if (_BracketsList.AsBytes().SequenceEqual(jumpTable.AsBytes()))
            {
                jumpTable.Dispose();
                return false;
            }

            // Update the current brackets sequence
            _BracketsList.Dispose();
            _BracketsList = jumpTable.Slice(jumps);

            return true;
        }
    }
}
