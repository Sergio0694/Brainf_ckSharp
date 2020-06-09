using JetBrains.Annotations;

namespace Brainf_ck_sharp.Legacy.UWP.DataModels.Misc.Themes
{
    /// <summary>
    /// A class that contains a <see cref="IDEThemeInfo"/> instance and indicates whether or not it is selected
    /// </summary>
    public sealed class SelectableIDEThemeInfo : SelectableModelBase<IDEThemeInfo>
    {
        /// <summary>
        /// Creates a new instance around the given <see cref="IDEThemeInfo"/>
        /// </summary>
        /// <param name="value">The theme to wrap</param>
        public SelectableIDEThemeInfo([NotNull] IDEThemeInfo value) : base(value) { }
    }
}
