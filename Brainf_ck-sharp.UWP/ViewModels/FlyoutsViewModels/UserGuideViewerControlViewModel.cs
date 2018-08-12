using System.Collections.ObjectModel;
using Brainf_ck_sharp_UWP.DataModels;
using Brainf_ck_sharp_UWP.Enums;
using Brainf_ck_sharp_UWP.ViewModels.Abstract.JumpList;

namespace Brainf_ck_sharp_UWP.ViewModels.FlyoutsViewModels
{
    public class UserGuideViewerControlViewModel : JumpListViewModelBase<UserGuideSection, UserGuideSection>
    {
        public UserGuideViewerControlViewModel()
        {
            // Load the user guide sections to show to the user
            Source = new ObservableCollection<JumpListGroup<UserGuideSection, UserGuideSection>>
            {
                new JumpListGroup<UserGuideSection, UserGuideSection>(UserGuideSection.Introduction, new[] { UserGuideSection.Introduction }),
                new JumpListGroup<UserGuideSection, UserGuideSection>(UserGuideSection.Samples, new[] { UserGuideSection.Samples }),
                new JumpListGroup<UserGuideSection, UserGuideSection>(UserGuideSection.PBrain, new[] { UserGuideSection.PBrain }),
                new JumpListGroup<UserGuideSection, UserGuideSection>(UserGuideSection.Debugging, new[] { UserGuideSection.Debugging })
            };
        }
    }
}
