namespace Brainf_ckSharp.Models.Opcodes.Interfaces
{
    /// <summary>
    /// An <see langword="interface"/> for a Brainf*ck/PBrain opcode
    /// </summary>
    internal interface IOpcode
    {
        /// <summary>
        /// The operator to execute
        /// </summary>
        byte Operator { get; }
    }
}
