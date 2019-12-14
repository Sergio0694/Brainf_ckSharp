namespace Brainf_ck_sharp.NET.Enums
{
    /// <summary>
    /// An <see langword="enum"/> that provides a compact representation of the available Brainf*ck/PBrain operators
    /// </summary>
    /// <remarks>Using this instead of a <see cref="char"/> saves 1 byte per operator and it's ever so slightly faster</remarks>
    public enum Operator : byte
    {
        /// <summary>
        /// The <see langword="+"/> operator, maps <see cref="Constants.Operators.Plus"/>
        /// </summary>
        Plus,

        /// <summary>
        /// The <see langword="-"/> operator, maps <see cref="Constants.Operators.Minus"/>
        /// </summary>
        Minus,

        /// <summary>
        /// The <see langword=">"/> operator, maps <see cref="Constants.Operators.ForwardPtr"/>
        /// </summary>
        ForwardPtr,

        /// <summary>
        /// The <see langword="&lt;"/> operator, maps <see cref="Constants.Operators.BackwardPtr"/>
        /// </summary>
        BackwardPtr,

        /// <summary>
        /// The <see langword="."/> operator, maps <see cref="Constants.Operators.PrintChar"/>
        /// </summary>
        PrintChar,

        /// <summary>
        /// The <see langword=","/> operator, maps <see cref="Constants.Operators.ReadChar"/>
        /// </summary>
        ReadChar,

        /// <summary>
        /// The <see langword="["/> operator, maps <see cref="Constants.Operators.LoopStart"/>
        /// </summary>
        LoopStart,

        /// <summary>
        /// The <see langword="]"/> operator, maps <see cref="Constants.Operators.LoopEnd"/>
        /// </summary>
        LoopEnd,

        /// <summary>
        /// The <see langword="("/> operator, maps <see cref="Constants.Operators.FunctionStart"/>
        /// </summary>
        FunctionStart,

        /// <summary>
        /// The <see langword=")"/> operator, maps <see cref="Constants.Operators.FunctionEnd"/>
        /// </summary>
        FunctionEnd,

        /// <summary>
        /// The <see langword=":"/> operator, maps <see cref="Constants.Operators.FunctionCall"/>
        /// </summary>
        FunctionCall
    }
}
