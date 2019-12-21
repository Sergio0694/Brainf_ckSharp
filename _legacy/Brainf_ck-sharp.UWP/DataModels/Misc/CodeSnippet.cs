using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.DataModels.Misc
{
    /// <summary>
    /// A <see langword="class"/> that represents a code snippet the user can use in the IDE
    /// </summary>
    public class CodeSnippet
    {
        /// <summary>
        /// Gets the title of the code snippet
        /// </summary>
        [NotNull]
        public string Title { get; }

        /// <summary>
        /// Gets the actual code stored as plain text
        /// </summary>
        [NotNull]
        public string Code { get; }

        /// <summary>
        /// Gets the cursor offset after inserting the snippet into the target source code
        /// </summary>
        public int CursorOffset { get; }

        /// <summary>
        /// Creates a new snippet with the input arguments
        /// </summary>
        /// <param name="title">The snippet title</param>
        /// <param name="code">The actual code snippet</param>
        /// <param name="offset">The final cursor offset</param>
        public CodeSnippet([NotNull] string title, [NotNull] string code, int? offset)
        {
            Title = title;
            Code = code;
            CursorOffset = offset ?? code.Length;
        }
    }
}
