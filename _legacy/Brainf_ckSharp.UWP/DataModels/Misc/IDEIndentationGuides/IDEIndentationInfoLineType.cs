namespace Brainf_ck_sharp.Legacy.UWP.DataModels.Misc.IDEIndentationGuides
{
    /// <summary>
    /// Indicates the kind of info shown next to a line in the IDE
    /// </summary>
    public enum IDEIndentationInfoLineType
    {
        OpenLoopBracket,
        OpenFunctionBracket,
        Straight,
        ClosedBracket,
        Empty,
        SelfContainedLoop,
        SelfContainedFunction
    }
}