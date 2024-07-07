namespace Brainf_ckSharp.Memory.Interfaces;

/// <summary>
/// An <see langword="interface"/> that is used to provide the fixed size of a given machine state
/// </summary>
internal interface IMachineStateSize
{
    /// <summary>
    /// Gets the size of the machine state.
    /// </summary>
    static abstract int Value { get; }
}
