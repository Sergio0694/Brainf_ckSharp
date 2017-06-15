using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brainf_ck_sharp_UWP.SQLiteDatabase;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.ViewModels
{
    public class SaveCodePromptFlyoutViewModel : ViewModelBase
    {
        private bool _NameValid;

        public bool NameValid
        {
            get => _NameValid;
            private set
            {
                if (Set(ref _NameValid, value))
                    ValidStatusChanged?.Invoke(this, value);
            }
        }

        public event EventHandler<bool> ValidStatusChanged;

        public async Task ValidateNameAsync([NotNull] String name)
        {
            NameValid = name.Length > 0 && await SQLiteManager.Instance.CheckExistingName(name);
        }
    }
}
