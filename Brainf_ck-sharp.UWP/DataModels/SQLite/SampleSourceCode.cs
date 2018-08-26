using Brainf_ck_sharp_UWP.Helpers.UI;
using SQLite.Net.Attributes;

namespace Brainf_ck_sharp_UWP.DataModels.SQLite
{
    /// <summary>
    /// A model for a sample source code that supports the automatic localization of its display name
    /// </summary>
    public class SampleSourceCode : SourceCode
    {
        /// <summary>
        /// Gets the display name for the current instance
        /// </summary>
        [NotNull]
        public override string Title
        {
            get
            {
                string localized = LocalizationManager.GetResource(Uid);
                return localized.Length > 0 ? localized : base.Title;
            }
            set => base.Title = value;
        }
    }
}
