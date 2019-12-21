namespace Brainf_ckSharp.Enums
{
    /// <summary>
    /// An <see langword="enum"/> that indicates an overflow mode for the interpreter
    /// </summary>
    public enum OverflowMode
    {
        /// <summary>
        /// Each cell must be in the [0..65535] value, with overflow not allowed
        /// </summary>
        UshortWithNoOverflow,

        /// <summary>
        /// Each cell must be in the [0..65535] value, with overflow allowed
        /// </summary>
        UshortWithOverflow,

        /// <summary>
        /// Each cell must be in the [0..255] value, with overflow not allowed
        /// </summary>
        ByteWithNoOverflow,

        /// <summary>
        /// Each cell must be in the [0..255] range, with overflow allowed
        /// </summary>
        ByteWithOverflow
    }
}
