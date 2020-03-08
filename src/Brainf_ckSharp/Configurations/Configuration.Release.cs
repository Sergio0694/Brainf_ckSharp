using System;
using System.Threading;

namespace Brainf_ckSharp.Configurations
{
    /// <summary>
    /// A model for a RELEASE configuration being built
    /// </summary>
    public readonly ref partial struct ReleaseConfiguration
    {
        /// <summary>
        /// The sequence of indices for the breakpoints to apply to the script
        /// </summary>
        public readonly ReadOnlySpan<int> Breakpoints;

        /// <summary>
        /// The token to cancel the monitoring of breakpoints
        /// </summary>
        public readonly CancellationToken DebugToken;
    }
}
