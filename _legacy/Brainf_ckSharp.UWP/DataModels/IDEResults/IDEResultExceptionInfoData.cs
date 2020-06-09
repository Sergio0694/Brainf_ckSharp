using Brainf_ck_sharp.Legacy.UWP.DataModels.Misc;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.Legacy.UWP.DataModels.IDEResults
{
    /// <summary>
    /// A model that exposes info on an exception thrown by a script run in the IDE
    /// </summary>
    public class IDEResultExceptionInfoData : IDEResultSectionDataBase
    {
        /// <summary>
        /// Gets the info on the exception type that was generated
        /// </summary>
        [NotNull]
        public ScriptExceptionInfo Info { get; }

        /// <summary>
        /// Creates a new instance that exposes info on a script exception
        /// </summary>
        /// <param name="info">The info to wrap in the new instance</param>
        public IDEResultExceptionInfoData([NotNull] ScriptExceptionInfo info) : base(IDEResultSection.ExceptionType)
        {
            Info = info;
        }
    }
}
