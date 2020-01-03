using System;

namespace Brainf_ckSharp.Uwp.Controls.Ide.Enums
{
    /// <summary>
    /// An <see langword="enum"/> that indicates the indentation type for a given element
    /// </summary>
    [Flags]
    internal enum IndentationType
    {
        /// <summary>
        /// The current element is full sized and leaves an open indentation level
        /// </summary>
        Open = 1,

        /// <summary>
        /// The current element is self contained and leaves no open level
        /// </summary>
        SelfContained = 1 << 1,

        /// <summary>
        /// The current element is both self contained and leaves an open indentation level
        /// </summary>
        SelfContainedAndContinuing = 1 << 2,

        /// <summary>
        /// The current element is full size
        /// </summary>
        FullSize = Open | SelfContainedAndContinuing,

        /// <summary>
        /// The current element closes an indentation level
        /// </summary>
        IsClosing = SelfContained | SelfContainedAndContinuing
    }
}
