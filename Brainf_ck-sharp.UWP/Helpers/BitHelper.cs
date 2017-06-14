using System;

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
        public static uint SetTo(uint value, bool bit, int n)
        {
            if (n < 0 || n > 31) throw new ArgumentOutOfRangeException("The position must be between 0 and 31");
            return bit 
                ? value | UnsignedOne << n 
                : value & ~(UnsignedOne << n);
        }

        /// <summary>
        /// Returns true or false depending on the value of the n-th bit in a number
        /// </summary>
        /// <param name="value">The number to check</param>
        /// <param name="n">The index of the bit to check</param>
        public static bool Test(uint value, int n)
        {
            if (n < 0 || n > 31) throw new ArgumentOutOfRangeException("The position must be between 0 and 31");
            uint bit = (value >> n) & 1;
            return bit == 1;
        }
    }
}
