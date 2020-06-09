using Brainf_ck_sharp.Legacy.UWP.DataModels.Misc;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.Legacy.UWP.DataModels.ConsoleModels
{
    /// <summary>
    /// Represents the exception info for a faulted script run in the console
    /// </summary>
    public sealed class ConsoleExceptionResult : ConsoleCommandModelBase
    {
        /// <summary>
        /// Gets the wrapped info to show
        /// </summary>
        [NotNull]
        public ScriptExceptionInfo Info { get; }

        /// <summary>
        /// Initializes a new instance for an exception that was generated
        /// </summary>
        /// <param name="info">The exception info</param>
        public ConsoleExceptionResult([NotNull] ScriptExceptionInfo info)
        {
            Info = info;
        }
    }
}
