using System.Collections.Generic;
using Microsoft.Toolkit.Mvvm.ComponentModel;

#nullable enable

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages
{
    /// <summary>
    /// A view model for the about page in the app
    /// </summary>
    public sealed class AboutSubPageViewModel : ViewModelBase
    {
        /// <summary>
        /// Forwards the <see cref="ThisAssembly.Git.Branch"/> property
        /// </summary>
        public string GitBranch => ThisAssembly.Git.Branch;

        /// <summary>
        /// Forwards the <see cref=" ThisAssembly.Git.Commit"/> property
        /// </summary>
        public string GitCommit => ThisAssembly.Git.Commit;

        private static IEnumerable<object>? _DonationMockupSource;

        /// <summary>
        /// Gets the mockup list to load the donation placeholder
        /// </summary>
        public IEnumerable<object>? DonationMockupSource
        {
            get => _DonationMockupSource;
            private set => Set(ref _DonationMockupSource, value);
        }
    }
}

