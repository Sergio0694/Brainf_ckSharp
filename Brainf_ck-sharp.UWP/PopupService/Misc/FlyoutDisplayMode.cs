namespace Brainf_ck_sharp_UWP.PopupService.Misc
{
    /// <summary>
    /// Indicates the display mode for a new flyout to show
    /// </summary>
    public enum FlyoutDisplayMode
    {
        /// <summary>
        /// The flyout will be displayed with the maximum possible size and the content will scroll
        /// </summary>
        ScrollableContent,

        /// <summary>
        /// The flyout will adapt to the size of its content, if possible
        /// </summary>
        ActualHeight
    }
}