using Brainf_ckSharp.Models;
using Brainf_ckSharp.Shared.Enums;

namespace Brainf_ckSharp.Uwp.Models.Ide.Views
{
    /// <summary>
    /// A simple model that associates a specific section to an <see cref="InterpreterResult"/> instance
    /// </summary>
    public sealed class IdeResultWithSectionInfo
    {
        /// <summary>
        /// Creates a new <see cref="IdeResultWithSectionInfo"/> instance with the specified parameters
        /// </summary>
        /// <param name="section">The current section being targeted</param>
        /// <param name="result">The <see cref="InterpreterResult"/> instance to wrap</param>
        public IdeResultWithSectionInfo(IdeResultSection section, InterpreterResult result)
        {
            Section = section;
            Result = result;
        }

        /// <summary>
        /// Gets the current section being targeted
        /// </summary>
        public IdeResultSection Section { get; }

        /// <summary>
        /// Gets the <see cref="InterpreterResult"/> instance currently wrapped
        /// </summary>
        public InterpreterResult Result { get; }
    }
}
