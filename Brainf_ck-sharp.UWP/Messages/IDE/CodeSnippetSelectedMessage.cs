using Windows.Devices.Input;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using Brainf_ck_sharp_UWP.Enums;
using Brainf_ck_sharp_UWP.Messages.Abstract;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Messages.IDE
{
    /// <summary>
    /// A message that signals whenever the user selects a code snippet to insert into the IDE
    /// </summary>
    public sealed class CodeSnippetSelectedMessage
    {
        /// <summary>
        /// Gets the <see cref="CodeSnippet"/> instance selected by the user
        /// </summary>
        [NotNull]
        public CodeSnippet Snippet { get; }

        /// <summary>
        /// Gets the pointer type used to make the snippet selection
        /// </summary>
        public PointerDeviceType Source { get; }

        /// <summary>
        /// Creates a new instance with the given parameters
        /// </summary>
        /// <param name="snippet">The snippet selected by the user</param>
        /// <param name="source">The pointer device used to select the code snippet</param>
        public CodeSnippetSelectedMessage([NotNull] CodeSnippet snippet, PointerDeviceType source)
        {
            Snippet = snippet;
            Source = source;
        }
    }
}
