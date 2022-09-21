namespace Brainf_ckSharp.Constants;

/// <summary>
/// A <see langword="class"/> that exposes the available operators for quick lookup
/// </summary>
internal static class Operators
{
    /// <summary>
    /// The <see langword="["/> operator, maps <see cref="Characters.LoopStart"/>
    /// </summary>
    /// <remarks>Loop operators come first for quicker lookup when building the jump table</remarks>
    public const byte LoopStart = 0;

    /// <summary>
    /// The <see langword="]"/> operator, maps <see cref="Characters.LoopEnd"/>
    /// </summary>
    public const byte LoopEnd = 1;

    /// <summary>
    /// The <see langword="("/> operator, maps <see cref="Characters.FunctionStart"/>
    /// </summary>
    public const byte FunctionStart = 2;

    /// <summary>
    /// The <see langword=")"/> operator, maps <see cref="Characters.FunctionEnd"/>
    /// </summary>
    public const byte FunctionEnd = 3;

    /// <summary>
    /// The <see langword="+"/> operator, maps <see cref="Characters.Plus"/>
    /// </summary>
    public const byte Plus = 4;

    /// <summary>
    /// The <see langword="-"/> operator, maps <see cref="Characters.Minus"/>
    /// </summary>
    public const byte Minus = 5;

    /// <summary>
    /// The <see langword=">"/> operator, maps <see cref="Characters.ForwardPtr"/>
    /// </summary>
    public const byte ForwardPtr = 6;

    /// <summary>
    /// The <see langword="&lt;"/> operator, maps <see cref="Characters.BackwardPtr"/>
    /// </summary>
    public const byte BackwardPtr = 7;

    /// <summary>
    /// The <see langword="."/> operator, maps <see cref="Characters.PrintChar"/>
    /// </summary>
    public const byte PrintChar = 8;

    /// <summary>
    /// The <see langword=","/> operator, maps <see cref="Characters.ReadChar"/>
    /// </summary>
    public const byte ReadChar = 9;

    /// <summary>
    /// The <see langword=":"/> operator, maps <see cref="Characters.FunctionCall"/>
    /// </summary>
    public const byte FunctionCall = 10;
}
