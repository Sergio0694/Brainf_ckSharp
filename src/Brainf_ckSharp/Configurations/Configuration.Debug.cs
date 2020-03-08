using System;
using System.Threading;

namespace Brainf_ckSharp.Configurations
{
    /// <summary>
    /// A model for a DEBUG configuration being built
    /// </summary>
    public readonly ref partial struct DebugConfiguration
    {
        /// <summary>
        /// The sequence of indices for the breakpoints to apply to the script
        /// </summary>
        public readonly ReadOnlyMemory<int> Breakpoints;

        /// <summary>
        /// The token to cancel the monitoring of breakpoints
        /// </summary>
        public readonly CancellationToken DebugToken;
    }
}
