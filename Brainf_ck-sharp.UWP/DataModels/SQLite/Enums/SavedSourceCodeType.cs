namespace Brainf_ck_sharp_UWP.DataModels.SQLite.Enums
{
    /// <summary>
    /// Indicates the type of saved source code
    /// </summary>
    public enum SavedSourceCodeType
    {
        /// <summary>
        /// A sample source code that can't be edited or deleted
        /// </summary>
        Sample,

        /// <summary>
        /// A source code created by the user and favorited
        /// </summary>
        Favorite,

        /// <summary>
        /// An original code saved by the user
        /// </summary>
        Original
    }
}