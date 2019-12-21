using Brainf_ckSharp.Legacy.Enums;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.DataModels.EventArgs
{
    /// <summary>
    /// The arguments for an event that signals a request to execute a source code
    /// </summary>
    public sealed class PlayRequestedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Gets the current stdin buffer
        /// </summary>
        [NotNull]
        public string Stdin { get; }

        /// <summary>
        /// Gets the overflow mode to use to execute the script
        /// </summary>
        public OverflowMode Mode { get; }

        /// <summary>
        /// Gets whether or not to execute the script in debug mode (stopping at each breakpoint)
        /// </summary>
        public bool Debug { get; }

        /// <summary>
        /// Creates a new instance for an event
        /// </summary>
        /// <param name="stdin">The stdin buffer</param>
        /// <param name="mode">The requested execution mode</param>
        /// <param name="debug">Indicates whether or not to execute the code in debug mode</param>
        public PlayRequestedEventArgs([NotNull] string stdin, OverflowMode mode, bool debug)
        {
            Stdin = stdin;
            Mode = mode;
            Debug = debug;
        }
    }
}