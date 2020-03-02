using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
    /// <summary>
    /// An extension <see langword="class"/> for the <see cref="int"/> type
    /// </summary>
    public static class IntExtensions
    {
        /// <summary>
        /// The map of hexadecimal characters for values in the [0, 15] range, as <see cref="byte"/>
        /// </summary>
        private static ReadOnlySpan<byte> HexCharactersMap => new[]
        {
            (byte)'0', (byte)'1', (byte)'2', (byte)'3',
            (byte)'4', (byte)'5', (byte)'6', (byte)'7',
            (byte)'8', (byte)'9', (byte)'A', (byte)'B',
            (byte)'C', (byte)'D', (byte)'E', (byte)'F'
        };

        /// <summary>
        /// Returns the padded hexadecimal representation of a given <see cref="int"/> value
        /// </summary>
        /// <param name="value">The input <see cref="int"/> value</param>
        /// <returns>The padded hexadecimal representation of <paramref name="value"/></returns>
        [Pure]
        public static unsafe string ToHex(this int value)
        {
            ref byte mapRef = ref MemoryMarshal.GetReference(HexCharactersMap);
            char* p = stackalloc char[sizeof(int) * 2];

            p[0] = (char)Unsafe.Add(ref mapRef, (int)(((uint)value & 0xF0000000) >> 28));
            p[1] = (char)Unsafe.Add(ref mapRef, (int)(((uint)value & 0x0F000000) >> 24));
            p[2] = (char)Unsafe.Add(ref mapRef, (int)(((uint)value & 0x00F00000) >> 20));
            p[3] = (char)Unsafe.Add(ref mapRef, (int)(((uint)value & 0x000F0000) >> 16));
            p[4] = (char)Unsafe.Add(ref mapRef, (int)(((uint)value & 0x0000F000) >> 12));
            p[5] = (char)Unsafe.Add(ref mapRef, (int)(((uint)value & 0x00000F00) >> 8));
            p[6] = (char)Unsafe.Add(ref mapRef, (int)(((uint)value & 0x000000F0) >> 4));
            p[7] = (char)Unsafe.Add(ref mapRef, (int)(((uint)value & 0x0000000F) >> 0));

            return new string(p, 0, sizeof(int) * 2);
        }
    }
}
