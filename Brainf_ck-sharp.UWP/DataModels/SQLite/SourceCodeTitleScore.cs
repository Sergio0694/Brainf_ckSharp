namespace Brainf_ck_sharp_UWP.DataModels.SQLite
{
    /// <summary>
    /// Indicates the result when checking if a title can be used to save a source code
    /// </summary>
    public enum SourceCodeTitleScore
    {
        Empty,
        AlreadyUsed,
        Valid
    }
}