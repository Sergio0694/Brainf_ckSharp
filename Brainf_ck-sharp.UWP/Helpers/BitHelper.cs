using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Helpers
{
    /// <summary>
    /// Class that contains some static methods to check/edit single bits in an unsigned int number
    /// </summary>
    public static class BitHelper
    {
        /// <summary>
        /// Just the value 1 as an unsigned int, used to avoid casting manually
        /// </summary>
        private const uint UnsignedOne = 1U;

        /// <summary>
        /// Sets to either 1 or 0 the target bit in a number
        /// </summary>
        /// <param name="value">The value to edit</param>
        /// <param name="bit">The value of the bit to edit</param>
        /// <param name="n">The position of the target bit</param>
        public static uint Set(this uint value, bool bit, int n)
        {
            if (n < 0 || n > 31) throw new ArgumentOutOfRangeException(nameof(n), "The position must be between 0 and 31");
            return bit 
                ? value | UnsignedOne << n 
                : value & ~(UnsignedOne << n);
        }

        /// <summary>
        /// Returns true or false depending on the value of the n-th bit in a number
        /// </summary>
        /// <param name="value">The number to check</param>
        /// <param name="n">The index of the bit to check</param>
        public static bool Test(this uint value, int n)
        {
            if (n < 0 || n > 31) throw new ArgumentOutOfRangeException(nameof(n), "The position must be between 0 and 31");
            uint bit = (value >> n) & 1;
            return bit == 1;
        }

        /// <summary>
        /// Compresses a collection of int values into a byte array, where each bit 
        /// set to 1 at the n-th position represents an int value in the input list
        /// </summary>
        /// <param name="values">The values to compress</param>
        public static byte[] Compress([NotNull] IReadOnlyCollection<int> values)
        {
            HashSet<int> set = new HashSet<int>(values);
            int max = values.Max();
            byte[] result = new byte[max % 8 == 0 ? max / 8 : max / 8 + 1];
            for (int i = 0; i < result.Length; i++)
            {
                byte b = 0;
                for (int j = 0; j < 8; j++)
                {
                    if (set.Contains(i * 8 + j)) b |= 0x1;
                    b <<= 1;
                }
                result[i] = b;
            }
            return result;
        }

        /// <summary>
        /// Expands a byte array in an array of int values where each one indicates the 0/1 flag at the n-th bit
        /// </summary>
        /// <param name="bits">The source bits to expand</param>
        public static IReadOnlyCollection<int> Expand([NotNull] byte[] bits)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < bits.Length; i++)
            {
                for (int j = 7; j >= 0; j--)
                {
                    if ((bits[i] >> j & 0x1) == 0x1)
                        result.Add(i * 8 + (7 - j));
                }
            }
            return result;
        }
    }
}
