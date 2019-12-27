using System;
using System.Collections.ObjectModel;
using Brainf_ckSharp.Uwp.Enums;
using Brainf_ckSharp.Uwp.ViewModels.Abstract;

namespace Brainf_ckSharp.Uwp.ViewModels.Controls.SubPages
{
    /// <summary>
    /// A view model for the user guide in the app
    /// </summary>
    public class UserGuideSubPageViewModel : GroupedItemsCollectionViewModelBase<UserGuideSection, UserGuideSection>
    {
        /// <summary>
        /// Creates a new <see cref="UserGuideSubPageViewModel"/>
        /// </summary>
        public UserGuideSubPageViewModel()
        {
            foreach (UserGuideSection section in Enum<UserGuideSection>.Values)
            {
                Source.Add(new ObservableGroup<UserGuideSection, UserGuideSection>(section, new[] { section }));
            }
        }
    }
}
