namespace Brainf_ckSharp.Services.Enums
{
    /// <summary>
    /// An <see langword="enum"/> indicating a CPU architecture
    /// </summary>
    public enum CpuArchitecture
    {
        /// <summary>
        /// The x86 architecture
        /// </summary>
        X86,

        /// <summary>
        /// The x64 architecture
        /// </summary>
        X64,

        /// <summary>
        /// The ARM32 architecture
        /// </summary>
        Arm,

        /// <summary>
        /// The ARM64 architecture
        /// </summary>
        Arm64,

        /// <summary>
        /// The WASM architecture
        /// </summary>
        Wasm,

        /// <summary>
        /// Unknown or not recognized architecture
        /// </summary>
        Unknown
    }
}
