using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace Brainf_ckSharp.Uwp.ViewModels.Abstract
{
    /// <summary>
    /// A view model that supports activation and deactivation
    /// </summary>
    public abstract class ReactiveViewModelBase : ViewModelBase
    {
        private bool _IsActive;

        /// <summary>
        /// Gets or sets whether the current view model is currently active
        /// </summary>
        public bool IsActive
        {
            get => _IsActive;
            set
            {
                if (Set(ref _IsActive, value))
                {
                    if (value) OnActivate();
                    else OnDeactivate();
                }
            }
        }

        /// <summary>
        /// Raised whenever the <see cref="IsActive"/> property is set to <see langword="true"/>
        /// </summary>
        protected virtual void OnActivate() { }

        /// <summary>
        /// Raised whenever the <see cref="IsActive"/> property is set to <see langword="false"/>
        /// </summary>
        /// <remarks>The base implementation unregisters all messages for this recipient</remarks>
        protected virtual void OnDeactivate() => Messenger.Default.Unregister(this);
    }
}
