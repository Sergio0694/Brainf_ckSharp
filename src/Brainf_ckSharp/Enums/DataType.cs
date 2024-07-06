namespace Brainf_ckSharp.Enums;

/// <summary>
/// An <see langword="enum"/> that indicates the data type to use for an interpreter
/// </summary>
public enum DataType
{
    /// <summary>
    /// Each cell is limited to the [0..255] range
    /// </summary>
    Byte,

    /// <summary>
    /// Each cell is limited to the [0..65535]
    /// </summary>
    UnsignedShort
}
