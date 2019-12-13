using System.Buffers;
using System.Runtime.CompilerServices;
using Brainf_ck_sharp.NET.Buffers;
using Brainf_ck_sharp.NET.Constants;
using Brainf_ck_sharp.NET.Models;

namespace Brainf_ck_sharp.NET
{
    /// <summary>
    /// A <see langword="class"/> responsible for interpreting and debugging Brainf*ck/PBrain scripts
    /// </summary>
    public static class Brainf_ckInterpreter
    {
        /// <summary>
        /// Loads the jump table for loops and functions from a given executable
        /// </summary>
        /// <param name="operators">The sequence of parsed operators to inspect</param>
        /// <param name="jumpTable">The resulting precomputed jump table for the input executable</param>
        private static void LoadJumpTable(UnsafeMemoryBuffer<Brainf_ckBinaryItem> operators, out UnsafeMemoryBuffer<int> jumpTable)
        {
            jumpTable = UnsafeMemoryBuffer<int>.Allocate(operators.Size);

            /* Temporarily allocate two buffers to store the indirect indices to build the jump table.
             * The two temporary buffers are initialized with a size of half the length of the input
             * executable, because that is the maximum number of open square brackets in a valid source file.
             * The two temporary buffers are used to implement an indirect indexing system while building
             * the table, which allows to reduce the complexity of the operation from O(N^2) to O(N) */
            int tempBuffersLength = operators.Size / 2 + 1;
            int[] rootTempIndices = ArrayPool<int>.Shared.Rent(tempBuffersLength);
            int[] functionTempIndices = ArrayPool<int>.Shared.Rent(tempBuffersLength);
            ref int rootTempIndicesRef = ref rootTempIndices[0];
            ref int functionTempIndicesRef = ref functionTempIndices[0];

            // Go through the executable to build the jump table for each open parenthesis or square bracket
            for (int r = 0, f = -1, i = 0; i < operators.Size; i++)
            {
                switch (operators[i].Operator)
                {
                    /* When a loop start, the current index is stored in the right
                     * temporary buffer, depending on whether or not the current
                     * part of the executable is within a function definition */
                    case Operators.LoopStart:
                        if (f == -1) Unsafe.Add(ref rootTempIndicesRef, r++) = i;
                        else Unsafe.Add(ref functionTempIndicesRef, f++) = i;
                        break;

                    /* When a loop ends, the index of the corresponding open
                     * square bracket is retrieved from the right temporary
                     * buffer, and the current index is stored at that location
                     * in the final jump table being built */
                    case Operators.LoopEnd:
                        if (f == -1) jumpTable[Unsafe.Add(ref rootTempIndicesRef, r--)] = i;
                        else jumpTable[Unsafe.Add(ref functionTempIndicesRef, f--)] = i;
                        break;

                    /* When a function definition starts, the offset into the
                     * temporary buffer for the function indices is set to 1.
                     * This is because in this case a 1-based indexing is used:
                     *the first location in the temporary buffer is used to store
                     * the index of the open parenthesis for the function definition */
                    case Operators.FunctionStart:
                        f = 1;
                        functionTempIndicesRef = i;
                        break;
                    case Operators.FunctionEnd:
                        f = -1;
                        jumpTable[functionTempIndicesRef] = i;
                        break;
                }
            }

            /* ArrayPool<T> is used directly here instead of UnsafeMemoryBuffer<T> to save the cost of
             * allocating a GCHandle and pinning each temporary buffer, which is not necessary since
             * both buffers are only used in the scope of this method, and then disposed */
            ArrayPool<int>.Shared.Return(rootTempIndices);
            ArrayPool<int>.Shared.Return(functionTempIndices);
        }
    }
}
