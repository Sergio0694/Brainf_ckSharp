using Brainf_ck_sharp.Legacy.UWP.DataModels.SQLite;
using Brainf_ck_sharp.Legacy.UWP.Messages.Abstract;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.Legacy.UWP.Messages.IDE
{
    /// <summary>
    /// A message that signals whenever the code the user is working on changes
    /// </summary>
    public sealed class WorkingSourceCodeChangedMessage : ValueChangedMessageBase<CategorizedSourceCode>
    {
        /// <inheritdoc cref="ValueChangedMessageBase{T}"/>
        public WorkingSourceCodeChangedMessage([CanBeNull] CategorizedSourceCode code) : base(code) { }
    }
}
