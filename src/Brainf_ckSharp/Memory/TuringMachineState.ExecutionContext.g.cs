using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Memory.Interfaces;
using Microsoft.Toolkit.HighPerformance.Extensions;

#pragma warning disable IDE0032

namespace Brainf_ckSharp.Memory
{
    /// <summary>
    /// A <see langword="class"/> that represents the state of a Turing machine
    /// </summary>
    internal sealed partial class TuringMachineState
    {
        /// <summary>
        /// Gets an execution context of the specified type
        /// </summary>
        /// <typeparam name="TExecutionContext">The type of execution context to retrieve</typeparam>
        /// <returns>An execution context of the specified type</returns>
        [Pure]
        private unsafe TExecutionContext GetExecutionContext<TExecutionContext>()
            where TExecutionContext : struct, IMachineStateExecutionContext
        {
            // The underlying buffer is guaranteed not to be null or unpinned here
            ushort* ptr = (ushort*)Unsafe.AsPointer(ref Buffer.DangerousGetReference());

            if (typeof(TExecutionContext) == typeof(ByteWithOverflowExecutionContext))
            {
                var executionContext = new ByteWithOverflowExecutionContext(ptr, Size - 1, _Position);

                return Unsafe.As<ByteWithOverflowExecutionContext, TExecutionContext>(ref executionContext);
            }

            if (typeof(TExecutionContext) == typeof(ByteWithNoOverflowExecutionContext))
            {
                var executionContext = new ByteWithNoOverflowExecutionContext(ptr, Size - 1, _Position);

                return Unsafe.As<ByteWithNoOverflowExecutionContext, TExecutionContext>(ref executionContext);
            }

            if (typeof(TExecutionContext) == typeof(UshortWithOverflowExecutionContext))
            {
                var executionContext = new UshortWithOverflowExecutionContext(ptr, Size - 1, _Position);

                return Unsafe.As<UshortWithOverflowExecutionContext, TExecutionContext>(ref executionContext);
            }

            if (typeof(TExecutionContext) == typeof(UshortWithNoOverflowExecutionContext))
            {
                var executionContext = new UshortWithNoOverflowExecutionContext(ptr, Size - 1, _Position);

                return Unsafe.As<UshortWithNoOverflowExecutionContext, TExecutionContext>(ref executionContext);
            }

            throw new ArgumentException($"Invalid context type: {typeof(TExecutionContext)}", nameof(TExecutionContext));
        }

        /// <summary>
        /// A <see langword="struct"/> implementing <see cref="IMachineStateExecutionContext"/> for <see cref="OverflowMode.ByteWithOverflow"/>
        /// </summary>
        public unsafe struct ByteWithOverflowExecutionContext : IMachineStateExecutionContext
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
            public int Position
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _Position;
            }

            /// <inheritdoc/>
            public ushort Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Ptr[_Position];
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
            public bool TryMoveNext(int count, ref int totalOperations)
            {
                if (_Position + count > MaxIndex)
                {
                    totalOperations += MaxIndex - _Position;

                    _Position = MaxIndex;

                    return false;
                }

                totalOperations += count;

                _Position += count;

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
            public bool TryMoveBack(int count, ref int totalOperations)
            {
                if (_Position - count < 0)
                {
                    totalOperations += _Position;

                    _Position = 0;

                    return false;
                }

                totalOperations += count;

                _Position -= count;

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
            public bool TryIncrement(int count, ref int totalOperations)
            {
                ushort* current = Ptr + _Position;

                *current = unchecked((byte)(*current + count));

                totalOperations += count;

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
            public bool TryDecrement(int count, ref int totalOperations)
            {
                ushort* current = Ptr + _Position;

                *current = unchecked((byte)(*current - count));

                totalOperations += count;

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

        /// <summary>
        /// A <see langword="struct"/> implementing <see cref="IMachineStateExecutionContext"/> for <see cref="OverflowMode.ByteWithNoOverflow"/>
        /// </summary>
        public unsafe struct ByteWithNoOverflowExecutionContext : IMachineStateExecutionContext
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
            public int Position
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _Position;
            }

            /// <inheritdoc/>
            public ushort Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Ptr[_Position];
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
            public bool TryMoveNext(int count, ref int totalOperations)
            {
                if (_Position + count > MaxIndex)
                {
                    totalOperations += MaxIndex - _Position;

                    _Position = MaxIndex;

                    return false;
                }

                totalOperations += count;

                _Position += count;

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
            public bool TryMoveBack(int count, ref int totalOperations)
            {
                if (_Position - count < 0)
                {
                    totalOperations += _Position;

                    _Position = 0;

                    return false;
                }

                totalOperations += count;

                _Position -= count;

                return true;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryIncrement()
            {
                ushort* current = Ptr + _Position;

                if (*current == byte.MaxValue) return false;

                *current = unchecked((byte)(*current + 1));

                return true;
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryIncrement(int count, ref int totalOperations)
            {
                ushort* current = Ptr + _Position;

                if (*current + count > byte.MaxValue)
                {
                    totalOperations += byte.MaxValue - *current;

                    *current = byte.MaxValue;

                    return false;
                }

                *current = unchecked((byte)(*current + count));

                totalOperations += count;

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
            public bool TryDecrement(int count, ref int totalOperations)
            {
                ushort* current = Ptr + _Position;

                if (*current < count)
                {
                    totalOperations += *current;

                    *current = 0;

                    return false;
                }

                *current = (ushort)(*current - count);

                totalOperations += count;

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
        /// A <see langword="struct"/> implementing <see cref="IMachineStateExecutionContext"/> for <see cref="OverflowMode.UshortWithOverflow"/>
        /// </summary>
        public unsafe struct UshortWithOverflowExecutionContext : IMachineStateExecutionContext
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
            public int Position
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _Position;
            }

            /// <inheritdoc/>
            public ushort Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Ptr[_Position];
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
            public bool TryMoveNext(int count, ref int totalOperations)
            {
                if (_Position + count > MaxIndex)
                {
                    totalOperations += MaxIndex - _Position;

                    _Position = MaxIndex;

                    return false;
                }

                totalOperations += count;

                _Position += count;

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
            public bool TryMoveBack(int count, ref int totalOperations)
            {
                if (_Position - count < 0)
                {
                    totalOperations += _Position;

                    _Position = 0;

                    return false;
                }

                totalOperations += count;

                _Position -= count;

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
            public bool TryIncrement(int count, ref int totalOperations)
            {
                ushort* current = Ptr + _Position;

                *current = unchecked((ushort)(*current + count));

                totalOperations += count;

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
            public bool TryDecrement(int count, ref int totalOperations)
            {
                ushort* current = Ptr + _Position;

                *current = unchecked((ushort)(*current - count));

                totalOperations += count;

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
        /// A <see langword="struct"/> implementing <see cref="IMachineStateExecutionContext"/> for <see cref="OverflowMode.UshortWithNoOverflow"/>
        /// </summary>
        public unsafe struct UshortWithNoOverflowExecutionContext : IMachineStateExecutionContext
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
            public int Position
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _Position;
            }

            /// <inheritdoc/>
            public ushort Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Ptr[_Position];
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
            public bool TryMoveNext(int count, ref int totalOperations)
            {
                if (_Position + count > MaxIndex)
                {
                    totalOperations += MaxIndex - _Position;

                    _Position = MaxIndex;

                    return false;
                }

                totalOperations += count;

                _Position += count;

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
            public bool TryMoveBack(int count, ref int totalOperations)
            {
                if (_Position - count < 0)
                {
                    totalOperations += _Position;

                    _Position = 0;

                    return false;
                }

                totalOperations += count;

                _Position -= count;

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
            public bool TryIncrement(int count, ref int totalOperations)
            {
                ushort* current = Ptr + _Position;

                if (*current + count > ushort.MaxValue)
                {
                    totalOperations += ushort.MaxValue - *current;

                    *current = ushort.MaxValue;

                    return false;
                }

                *current = unchecked((ushort)(*current + count));

                totalOperations += count;

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
            public bool TryDecrement(int count, ref int totalOperations)
            {
                ushort* current = Ptr + _Position;

                if (*current < count)
                {
                    totalOperations += *current;

                    *current = 0;

                    return false;
                }

                *current = (ushort)(*current - count);

                totalOperations += count;

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
    }
}
