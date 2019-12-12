using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Brainf_ck_sharp.NET.MemoryState;

#pragma warning disable IDE0032

namespace Brainf_ck_sharp.NET.Models
{
    /// <summary>
    /// A class that represents the state of a Touring machine (data + position)
    /// </summary>
    internal sealed unsafe class TouringMachineState : IDisposable
    {
        /// <summary>
        /// The size of the usable buffer within <see cref="Memory"/>
        /// </summary>
        private readonly int Size;

        /// <summary>
        /// The unsigned int array that represents the memory of the Touring machine
        /// </summary>
        private readonly ushort[] Memory;

        /// <summary>
        /// A pointer to the first element in <see cref="Memory"/>
        /// </summary>
        private readonly ushort* Ptr;

        /// <summary>
        /// The <see cref="GCHandle"/> instance used to pin <see cref="Memory"/>
        /// </summary>
        /// <remarks>This field is not <see langword="readonly"/> to prevent the safe copy when calling <see cref="GCHandle.Free"/> from <see cref="Dispose"/></remarks>
        private GCHandle _Handle;

        /// <summary>
        /// The current position within <see cref="Memory"/>
        /// </summary>
        private int _Position;

        /// <summary>
        /// Creates a new blank machine state with the given parameters
        /// </summary>
        /// <param name="size">The size of the new memory buffer to use</param>
        public TouringMachineState(int size)
        {
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size), "The size must be a positive number");
            if (size > 4096) throw new ArgumentOutOfRangeException(nameof(size), "The size can't be greater than 4096");

            Size = size;
            Memory = ArrayPool<ushort>.Shared.Rent(size);
            _Handle = GCHandle.Alloc(Memory, GCHandleType.Pinned);
            Ptr = (ushort*)Unsafe.AsPointer(ref Memory[0]);
        }

        /// <summary>
        /// Gets the current position on the memory buffer
        /// </summary>
        public int Position => _Position;

        /// <summary>
        /// Gets the total number of cells in the current memory buffer
        /// </summary>
        public int Count => Size;

        /// <summary>
        /// Gets the cell at the current memory position
        /// </summary>
        public Brainf_ckMemoryCell Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Brainf_ckMemoryCell(Ptr[_Position], true);
        }

        /// <summary>
        /// Gets the cell at the specified location from the current memory buffer
        /// </summary>
        /// <param name="index">The target index to read the valaue from</param>
        public Brainf_ckMemoryCell this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index < 0 || index >= Size) throw new ArgumentOutOfRangeException(nameof(index), "The input index was invalid");

                return new Brainf_ckMemoryCell(Ptr[index], _Position == index);
            }
        }

        /// <summary>
        /// Sets the current memory location to the value of a given character
        /// </summary>
        /// <param name="c">The input charachter to assign to the current memory location</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Input(char c) => Ptr[_Position] = c;

        /// <summary>
        /// Checks whether or not it is possible to move the pointer forward
        /// </summary>
        public bool CanMoveNext
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _Position < Size - 1;
        }

        /// <summary>
        /// Moves the memory pointer forward
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MoveNext() => _Position++;

        /// <summary>
        /// Checks whether or not it is possible to move the pointer back
        /// </summary>
        public bool CanMoveBack
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _Position > 0;
        }

        /// <summary>
        /// Moves the memory pointer back
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MoveBack() => _Position--;

        /// <summary>
        /// Increments the current memory location
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Plus() => Ptr[_Position]++;

        /// <summary>
        /// Checks whether or not it is possible to increment the current memory location
        /// </summary>
        public bool CanIncrement
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Ptr[_Position] < ushort.MaxValue;
        }

        /// <summary>
        /// Decrements the current memory location
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Minus() => Ptr[_Position]--;

        /// <summary>
        /// Checks whether or not the current memory location is a positive number and can be decremented
        /// </summary>
        public bool CanDecrement
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Ptr[_Position] > 0;
        }

        /// <summary>
        /// Resets the value in the current memory cell
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResetCell() => Ptr[_Position] = 0;

        /// <summary>
        /// Invokes <see cref="Dispose"/> to free the aallocated resources when this instance is destroyed
        /// </summary>
        ~TouringMachineState() => Dispose();

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!_Handle.IsAllocated) return;

            _Handle.Free();
            ArrayPool<ushort>.Shared.Return(Memory);

            GC.SuppressFinalize(this);
        }
    }
}
