namespace Brainf_ckSharp.Uwp.Enums
{
    /// <summary>
    /// An <see cref="object"/> based <see langword="enum"/> that indicates a section of the app user guide
    /// </summary>
    public sealed class UserGuideSection
    {
        /// <summary>
        /// The introduction section, with general info on the language and its operators
        /// </summary>
        public static readonly UserGuideSection Introduction = new UserGuideSection();

        /// <summary>
        /// A section with some simple code samples
        /// </summary>
        public static readonly UserGuideSection Samples = new UserGuideSection();

        /// <summary>
        /// A section with info on the PBrain language extension
        /// </summary>
        public static readonly UserGuideSection PBrain = new UserGuideSection();

        /// <summary>
        /// A section on the use of breaakpoints to debug code
        /// </summary>
        public static readonly UserGuideSection Debugging = new UserGuideSection();
    }
}
