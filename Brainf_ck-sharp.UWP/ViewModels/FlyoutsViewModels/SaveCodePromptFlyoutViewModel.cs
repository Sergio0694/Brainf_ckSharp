using System;
using System.Threading.Tasks;
using Brainf_ck_sharp_UWP.DataModels.SQLite.Enums;
using Brainf_ck_sharp_UWP.SQLiteDatabase;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.ViewModels.FlyoutsViewModels
{
    public class SaveCodePromptFlyoutViewModel : ViewModelBase
    {
        /// <inheritdoc cref="ViewModelBase.Cleanup"/>
        public override void Cleanup()
        {
            base.Cleanup();
            ValidStatusChanged = null;
        }

        /// <summary>
        /// Gets the current name for the code, if it's already saved in the app
        /// </summary>
        private readonly string OriginalName;

        public SaveCodePromptFlyoutViewModel([CanBeNull] string name) => OriginalName = name;

        private bool _NameValid;

        /// <summary>
        /// Gets whether or not the current name is valid
        /// </summary>
        public bool NameValid
        {
            get => _NameValid;
            private set
            {
                if (Set(ref _NameValid, value))
                    ValidStatusChanged?.Invoke(this, value);
            }
        }

        private SourceCodeTitleScore _NameScore;

        /// <summary>
        /// Gets the current score for the name candidate
        /// </summary>
        public SourceCodeTitleScore NameScore
        {
            get => _NameScore;
            private set => Set(ref _NameScore, value);
        }

        /// <summary>
        /// Raised whenever the status changes for the current source code title
        /// </summary>
        public event EventHandler<bool> ValidStatusChanged;

        /// <summary>
        /// Updates the status for a name candidate for a source code to save
        /// </summary>
        /// <param name="name">The desired name to use to save the source code</param>
        public async Task ValidateNameAsync([NotNull] string name)
        {
            if (name.Length == 0)
            {
                NameValid = false;
                NameScore = SourceCodeTitleScore.Empty;
            }
            else
            {
                NameValid = await SQLiteManager.Instance.CheckExistingName(name);
                NameScore = NameValid 
                    ? SourceCodeTitleScore.Valid 
                    : (OriginalName?.Equals(name) == true ? SourceCodeTitleScore.NotModified : SourceCodeTitleScore.AlreadyUsed);
            }
        }
    }
}
