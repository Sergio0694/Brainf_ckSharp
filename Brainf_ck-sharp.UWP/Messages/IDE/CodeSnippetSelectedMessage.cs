using Brainf_ck_sharp_UWP.DataModels.Misc;
using Brainf_ck_sharp_UWP.Messages.Abstract;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Messages.IDE
{
    /// <summary>
    /// A message that signals whenever the user selects a code snippet to insert into the IDE
    /// </summary>
    public sealed class CodeSnippetSelectedMessage : ValueChangedMessageBase<CodeSnippet>
    {
        /// <inheritdoc cref="ValueChangedMessageBase{T}"/>
        public CodeSnippetSelectedMessage([NotNull] CodeSnippet snippet) : base(snippet) { }
    }
}
