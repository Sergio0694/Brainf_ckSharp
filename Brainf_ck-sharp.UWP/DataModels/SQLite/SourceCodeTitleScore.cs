namespace Brainf_ck_sharp_UWP.DataModels.SQLite
{
    /// <summary>
    /// Indicates the result when checking if a title can be used to save a source code
    /// </summary>
    public enum SourceCodeTitleScore
    {
        Empty,
        AlreadyUsed,

        /// <summary>
        /// Indicates that the desired name is the same as the one already in use for a given saved code
        /// </summary>
        NotModified,
        Valid
    }
}