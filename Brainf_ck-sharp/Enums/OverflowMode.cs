namespace Brainf_ck_sharp.Enums
{
    /// <summary>
    /// Indicates an overflow mode for the interpreter
    /// </summary>
    public enum OverflowMode
    {
        /// <summary>
        /// Each cell must be in the [0..65535] value, with no overflow allowed
        /// </summary>
        ShortNoOverflow,

        /// <summary>
        /// Each cell must be in the [0..255] range, with overflow permitted
        /// </summary>
        ByteOverflow
    }
}