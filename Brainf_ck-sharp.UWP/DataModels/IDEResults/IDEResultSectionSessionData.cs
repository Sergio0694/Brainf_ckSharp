using Brainf_ck_sharp.ReturnTypes;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.DataModels.IDEResults
{
    /// <summary>
    /// A model that exposes general info for a script run in the IDE
    /// </summary>
    public class IDEResultSectionSessionData : IDEResultSectionDataBase
    {
        /// <summary>
        /// Gets the current aggregate info for the script that was run
        /// </summary>
        [NotNull]
        public InterpreterExecutionSession Session { get; }

        /// <summary>
        /// Creates a new instance that exposes a given kind of info
        /// </summary>
        /// <param name="section">The kind of info to expose in this instance</param>
        /// <param name="session">The aggregate info to store in the new instance</param>
        public IDEResultSectionSessionData(IDEResultSection section, [NotNull] InterpreterExecutionSession session) : base(section)
        {
            Session = session;
        }
    }
}
