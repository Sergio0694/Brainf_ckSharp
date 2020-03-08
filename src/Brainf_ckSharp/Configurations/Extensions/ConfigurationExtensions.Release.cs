using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using Brainf_ckSharp.Configurations;

namespace Brainf_ckSharp
{
    /// <summary>
    /// Extensions for building RELEASE configurations
    /// </summary>
    public static partial class ReleaseConfigurationExtensions
    {
        /// <summary>
        /// Sets the source code to parse and execute for a given configuration
        /// </summary>
        /// <param name="configuration">The input <see cref="ReleaseConfiguration"/> instance</param>
        /// <param name="breakpoints">The sequence of indices for the breakpoints to apply to the script</param>
        /// <returns>The input <see cref="ReleaseConfiguration"/> instance</returns>
        [Pure]
        public static ref readonly ReleaseConfiguration WithBreakpoints(in this ReleaseConfiguration configuration, ReadOnlyMemory<int> breakpoints)
        {
            Unsafe.AsRef(configuration.Breakpoints) = breakpoints;

            return ref configuration;
        }

        /// <summary>
        /// Sets the debug token for a given configuration
        /// </summary>
        /// <param name="configuration">The input <see cref="ReleaseConfiguration"/> instance</param>
        /// <param name="debugToken">A <see cref="CancellationToken"/> that is used to ignore/respect existing breakpoints</param>
        /// <returns>The input <see cref="ReleaseConfiguration"/> instance</returns>
        [Pure]
        public static ref readonly ReleaseConfiguration WithDebugToken(in this ReleaseConfiguration configuration, CancellationToken debugToken)
        {
            Unsafe.AsRef(configuration.DebugToken) = debugToken;

            return ref configuration;
        }
    }
}
