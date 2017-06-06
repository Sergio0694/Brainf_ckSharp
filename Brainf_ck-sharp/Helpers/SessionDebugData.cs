using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.Helpers
{
    /// <summary>
    /// A class that holds additional informations needed to preserve the state of an execution session
    /// </summary>
    internal sealed class SessionDebugData
    {
        /// <summary>
        /// Gets the original source code for the script used to initialize the session
        /// </summary>
        public IReadOnlyList<Brainf_ckBinaryItem> Source { get; }

        /// <summary>
        /// Gets the current stdin for the session
        /// </summary>
        public Queue<char> Stdin { get; }

        /// <summary>
        /// Gets the current stdout for the session
        /// </summary>
        public StringBuilder Stdout { get; }

        /// <summary>
        /// Gets the list of breakpoints in the current session
        /// </summary>
        public IReadOnlyList<uint> Breakpoints { get; }

        /// <summary>
        /// Gets the original optional threshold used to initialize the session
        /// </summary>
        public int? Threshold { get; }

        /// <summary>
        /// Creates a new instance to store the state of the current session
        /// </summary>
        /// <param name="source">The source code for the session</param>
        /// <param name="stdin">The current stdin buffer</param>
        /// <param name="stdout">The current stdout buffer</param>
        /// <param name="threshold">The optional time threshold</param>
        /// <param name="breakpoints">The original list of breakpoints</param>
        public SessionDebugData([NotNull] IReadOnlyList<Brainf_ckBinaryItem> source, [NotNull] Queue<char> stdin, 
            [NotNull] StringBuilder stdout, int? threshold, IReadOnlyList<uint> breakpoints)
        {
            Source = source;
            Stdin = stdin;
            Stdout = stdout;
            Threshold = threshold;
            Breakpoints = breakpoints;
        }

        /// <summary>
        /// Creates a copy of the current deug data
        /// </summary>
        [Pure, NotNull]
        public SessionDebugData Clone()
        {
            return new SessionDebugData(Source, new Queue<char>(Stdin), new StringBuilder(Stdout.ToString()), Threshold, Breakpoints);
        }
    }
}
