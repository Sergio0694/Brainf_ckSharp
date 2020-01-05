using Brainf_ckSharp.Uwp.Controls.Ide.Enums;

namespace Brainf_ckSharp.Uwp.Controls.Ide.Models.Abstract
{
    /// <summary>
    /// A base model for all indentation indicators used in the IDE
    /// </summary>
    internal abstract class IndentationIndicatorBase
    {
        /// <summary>
        /// Gets or sets the <see cref="IndentationType"/> value for the current instance
        /// </summary>
        public IndentationType Type { get; set; }
    }
}
