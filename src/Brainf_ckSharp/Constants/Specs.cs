using Brainf_ckSharp.Enums;

namespace Brainf_ckSharp.Constants
{
    /// <summary>
    /// A <see langword="class"/> that exposes constants used by the library, and part of the specification for the interpreter
    /// </summary>
    internal static class Specs
    {
        /// <summary>
        /// The minimum allowed memory size
        /// </summary>
        public const int MinimumMemorySize = 32;

        /// <summary>
        /// The maximum allowed memory size
        /// </summary>
        public const int MaximumMemorySize = 1024 * 64;

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

        /// <summary>
        /// The maximum allowed size for the output buffer
        /// </summary>
        public const int StdoutBufferSizeLimit = 1024 * 8;
    }
}
