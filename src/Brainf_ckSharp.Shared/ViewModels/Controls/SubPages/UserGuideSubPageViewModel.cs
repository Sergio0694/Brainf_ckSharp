using System;
using Brainf_ckSharp.Shared.Enums;
using Brainf_ckSharp.Shared.ViewModels.Abstract;
using Microsoft.Toolkit.Collections;

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages
{
    /// <summary>
    /// A view model for the user guide in the app
    /// </summary>
    public sealed class UserGuideSubPageViewModel : ViewModelBase<ObservableGroupedCollection<UserGuideSection, UserGuideSection>>
    {
        /// <summary>
        /// The collection of available user guide sections
        /// </summary>
        private static readonly ReadOnlyMemory<UserGuideSection> UserGuideSections = new[]
        {
            UserGuideSection.Introduction,
            UserGuideSection.Samples,
            UserGuideSection.PBrain,
            UserGuideSection.Debugging
        };

        /// <summary>
        /// Creates a new <see cref="UserGuideSubPageViewModel"/>
        /// </summary>
        public UserGuideSubPageViewModel()
        {
            foreach (UserGuideSection section in UserGuideSections.Span)
            {
                Source.Add(new ObservableGroup<UserGuideSection, UserGuideSection>(section, new[] { section }));
            }
        }
    }
}
