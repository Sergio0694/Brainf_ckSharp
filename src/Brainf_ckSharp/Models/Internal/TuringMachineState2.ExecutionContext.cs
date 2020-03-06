using System.Runtime.CompilerServices;
using Brainf_ckSharp.Enums;

#pragma warning disable IDE0032

namespace Brainf_ckSharp.Models.Internal
{
    /// <summary>
    /// A <see langword="class"/> that represents the state of a Turing machine
    /// </summary>
    internal sealed unsafe partial class TuringMachineState2
    {
        /// <summary>
        /// A <see langword="struct"/> implementing <see cref="IMachineStateExecutionContext"/> for <see cref="OverflowMode.UshortWithNoOverflow"/>
        /// </summary>
        public struct UshortWithNoOverflowExecutionContext : IMachineStateExecutionContext
        {
            private readonly ushort* Ptr;
            private readonly int MaxIndex;
            private int _Position;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public UshortWithNoOverflowExecutionContext(ushort* ptr, int maxIndex, int position)
            {
                Ptr = ptr;
                MaxIndex = maxIndex;
                _Position = position;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryMoveNext()
            {
                if (_Position == MaxIndex) return false;

                _Position++;

                return true;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryMoveBack()
            {
                if (_Position == 0) return false;

                _Position--;

                return true;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryIncrement()
            {
                ushort* current = Ptr + _Position;

                if (*current == ushort.MaxValue) return false;

                *current = unchecked((ushort)(*current + 1));

                return true;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryDecrement()
            {
                ushort* current = Ptr + _Position;

                if (*current == 0) return false;

                *current = (ushort)(*current - 1);

                return true;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryInput(char c)
            {
                ushort* current = Ptr + _Position;

                *current = c;

                return true;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void ResetCell() => Ptr[_Position] = 0;
        }
        /// <summary>
        /// A <see langword="struct"/> implementing <see cref="IMachineStateExecutionContext"/> for <see cref="OverflowMode.UshortWithOverflow"/>
        /// </summary>
        public struct UshortWithOverflowExecutionContext : IMachineStateExecutionContext
        {
            private readonly ushort* Ptr;
            private readonly int MaxIndex;
            private int _Position;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public UshortWithOverflowExecutionContext(ushort* ptr, int maxIndex, int position)
            {
                Ptr = ptr;
                MaxIndex = maxIndex;
                _Position = position;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryMoveNext()
            {
                if (_Position == MaxIndex) return false;

                _Position++;

                return true;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryMoveBack()
            {
                if (_Position == 0) return false;

                _Position--;

                return true;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryIncrement()
            {
                ushort* current = Ptr + _Position;

                *current = unchecked((ushort)(*current + 1));

                return true;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryDecrement()
            {
                ushort* current = Ptr + _Position;

                *current = unchecked((ushort)(*current - 1));

                return true;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryInput(char c)
            {
                ushort* current = Ptr + _Position;

                *current = c;

                return true;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void ResetCell() => Ptr[_Position] = 0;
        }
        /// <summary>
        /// A <see langword="struct"/> implementing <see cref="IMachineStateExecutionContext"/> for <see cref="OverflowMode.ByteWithNoOverflow"/>
        /// </summary>
        public struct ByteWithNoOverflowExecutionContext : IMachineStateExecutionContext
        {
            private readonly ushort* Ptr;
            private readonly int MaxIndex;
            private int _Position;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ByteWithNoOverflowExecutionContext(ushort* ptr, int maxIndex, int position)
            {
                Ptr = ptr;
                MaxIndex = maxIndex;
                _Position = position;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryMoveNext()
            {
                if (_Position == MaxIndex) return false;

                _Position++;

                return true;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryMoveBack()
            {
                if (_Position == 0) return false;

                _Position--;

                return true;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryIncrement()
            {
                ushort* current = Ptr + _Position;

                if (*current == ushort.MaxValue) return false;

                *current = unchecked((byte)(*current + 1));

                return true;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryDecrement()
            {
                ushort* current = Ptr + _Position;

                if (*current == 0) return false;

                *current = (ushort)(*current - 1);

                return true;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryInput(char c)
            {
                ushort* current = Ptr + _Position;

                if (c > byte.MaxValue) return false;

                *current = c;

                return true;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void ResetCell() => Ptr[_Position] = 0;
        }
        /// <summary>
        /// A <see langword="struct"/> implementing <see cref="IMachineStateExecutionContext"/> for <see cref="OverflowMode.ByteWithOverflow"/>
        /// </summary>
        public struct ByteWithOverflowExecutionContext : IMachineStateExecutionContext
        {
            private readonly ushort* Ptr;
            private readonly int MaxIndex;
            private int _Position;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ByteWithOverflowExecutionContext(ushort* ptr, int maxIndex, int position)
            {
                Ptr = ptr;
                MaxIndex = maxIndex;
                _Position = position;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryMoveNext()
            {
                if (_Position == MaxIndex) return false;

                _Position++;

                return true;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryMoveBack()
            {
                if (_Position == 0) return false;

                _Position--;

                return true;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryIncrement()
            {
                ushort* current = Ptr + _Position;

                *current = unchecked((byte)(*current + 1));

                return true;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryDecrement()
            {
                ushort* current = Ptr + _Position;

                *current = unchecked((byte)(*current - 1));

                return true;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryInput(char c)
            {
                ushort* current = Ptr + _Position;

                *current = unchecked((byte)c);

                return true;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void ResetCell() => Ptr[_Position] = 0;
        }
    }
}
