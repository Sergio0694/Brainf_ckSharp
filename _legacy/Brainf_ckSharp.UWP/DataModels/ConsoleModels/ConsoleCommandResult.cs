using JetBrains.Annotations;

namespace Brainf_ck_sharp.Legacy.UWP.DataModels.ConsoleModels
{
    /// <summary>
    /// Represents the text output for an interpreter script that was run correctly
    /// </summary>
    public sealed class ConsoleCommandResult : ConsoleCommandModelBase
    {
        /// <summary>
        /// Gets the result for the current instance
        /// </summary>
        [NotNull]
        public string Result { get; }

        /// <summary>
        /// Initializes a new instance with the given result
        /// </summary>
        /// <param name="result">The result to display</param>
        public ConsoleCommandResult([NotNull] string result) => Result = result;
    }
}
