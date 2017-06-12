namespace Brainf_ck_sharp_UWP.DataModels.IDEResults
{
    /// <summary>
    /// Indicates the kind of info to show about an interpreter session
    /// </summary>
    public enum IDEResultSection
    {
        ExceptionType,
        Stdout,
        ErrorLocation,
        BreakpointReached,
        StackTrace,
        SourceCode,
        MemoryState,
        Stats
    }
}