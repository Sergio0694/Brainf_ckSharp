namespace Brainf_ckSharp.Uwp.Enums
{
    /// <summary>
    /// An <see cref="object"/> based <see langword="enum"/> that indicates a section of the code library
    /// </summary>
    /// <remarks>This type is not a value type to avoid repeated boxing</remarks>
    public sealed class CodeLibraryCategory
    {
        /// <summary>
        /// The favorited source codes
        /// </summary>
        public static readonly CodeLibraryCategory Favorites = new CodeLibraryCategory();

        /// <summary>
        /// The recently used source code files
        /// </summary>
        public static readonly CodeLibraryCategory Recent = new CodeLibraryCategory();

        /// <summary>
        /// The available sample source codes
        /// </summary>
        public static readonly CodeLibraryCategory Samples = new CodeLibraryCategory();
    }
}
