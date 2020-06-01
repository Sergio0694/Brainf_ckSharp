namespace Brainf_ckSharp.Shared.Models.Ide
{
    /// <summary>
    /// A <see langword="class"/> that represents a code snippet the user can use in the IDE
    /// </summary>
    public sealed class CodeSnippet
    {
        /// <summary>
        /// Creates a new <see cref="CodeSnippet"/> instance with the specified parameters
        /// </summary>
        /// <param name="title">The snippet title</param>
        /// <param name="code">The actual code snippet</param>
        public CodeSnippet(string title, string code)
        {
            Title = title;
            Code = code;
        }

        /// <summary>
        /// Gets the title of the code snippet
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the actual code stored as plain text
        /// </summary>
        public string Code { get; }
    }
}
