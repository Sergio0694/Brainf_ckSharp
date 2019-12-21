namespace Brainf_ck_sharp_UWP.PopupService.Misc
{
    /// <summary>
    /// Represents the result of a closed popup
    /// </summary>
    public struct FlyoutClosedResult<T>
    {
        /// <summary>
        /// Gets an instance that represents the result of a flyout closed without confirm
        /// </summary>
        public static FlyoutClosedResult<T> Closed { get; } = new FlyoutClosedResult<T>(FlyoutResult.Canceled, default);

        /// <summary>
        /// Gets the inner enum value of the instance
        /// </summary>
        public FlyoutResult Result { get; }

        /// <summary>
        /// Gets the return value for the flyout, if confirmed
        /// </summary>
        public T Value { get; }

        public FlyoutClosedResult(FlyoutResult result, T value)
        {
            Result = result;
            Value = value;
        }

        // Implicit converter for a successful flyout
        public static implicit operator FlyoutClosedResult<T>(T value) => new FlyoutClosedResult<T>(FlyoutResult.Confirmed, value);

        // Converts an instance into a bool value that indicates whether or not the popup was confirmed
        public static implicit operator bool(FlyoutClosedResult<T> result) => result.Result == FlyoutResult.Confirmed;
    }

    /// <summary>
    /// Indicates the result of a closed flyout
    /// </summary>
    public enum FlyoutResult
    {
        Confirmed,
        Canceled
    }
}