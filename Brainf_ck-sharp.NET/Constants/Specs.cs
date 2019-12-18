using Brainf_ck_sharp.NET.Enums;

namespace Brainf_ck_sharp.NET.Constants
{
    /// <summary>
    /// A <see langword="class"/> that exposes constants used by the library, and part of the specification for the interpreter
    /// </summary>
    internal static class Specs
    {
        /// <summary>
        /// The default memory size for machine states used to run scripts
        /// </summary>
        public const int DefaultMemorySize = 128;

        /// <summary>
        /// The default overflow mode for running scripts
        /// </summary>
        public const OverflowMode DefaultOverflowMode = OverflowMode.ByteWithNoOverflow;

        /// <summary>
        /// The maximum number of recursive calls that can be performed by a script
        /// </summary>
        public const int MaximumStackSize = 512;
    }
}
