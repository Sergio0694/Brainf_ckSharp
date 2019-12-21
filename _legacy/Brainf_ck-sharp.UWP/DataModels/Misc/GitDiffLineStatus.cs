namespace Brainf_ck_sharp.Legacy.UWP.DataModels.Misc
{
    /// <summary>
    /// Indicates the state of a given line in a source code being monitored by the Git diff engine
    /// </summary>
    public enum GitDiffLineStatus
    {
        Undefined,
        Edited,
        Saved
    }
}