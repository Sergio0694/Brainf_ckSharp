using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Views
{
    /// <summary>
    /// An <see langword="interface"/> for a page that lets the user work on some source code
    /// </summary>
    public interface ICodeWorkspacePage
    {
        /// <summary>
        /// Gets the current source code the user is working on
        /// </summary>
        [NotNull]
        string SourceCode { get; }
    }
}