using System;

namespace Brainf_ckSharp.Enums;

/// <summary>
/// An <see langword="enum"/> that indicates the options to use to execute code
/// </summary>
[Flags]
public enum ExecutionOptions
{
    /// <summary>
    /// No options specified
    /// </summary>
    None,

    /// <summary>
    /// Allow each memory cell to overflow when incremented or decremented.
    /// </summary>
    AllowOverflow = 1 << 0
}
