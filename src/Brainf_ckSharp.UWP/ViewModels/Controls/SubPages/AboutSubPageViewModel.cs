using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GitHub.APIs;
using Microsoft.Toolkit.Uwp.Helpers;
using User = GitHub.Models.User;

namespace Legere.ViewModels.SubPages.Shell
{
    public sealed class AboutSubPageViewModel : ViewModelBase
    {
        /// <summary>
        /// Forwards the <see cref="SystemInformation.ApplicationVersion"/> property
        /// </summary>
        public string AppVersion => SystemInformation.ApplicationVersion.ToFormattedString();

        /// <summary>
        /// Forwards the <see cref="PackageId.Architecture"/> property
        /// </summary>
        public ProcessorArchitecture PackageArchitecture => Package.Current.Id.Architecture;

        /// <summary>
        /// Forwards the <see cref="ThisAssembly.Git.Branch"/> property
        /// </summary>
        public string GitBranch => ThisAssembly.Git.Branch;

        /// <summary>
        /// Forwards the <see cref=" ThisAssembly.Git.Commit"/> property
        /// </summary>
        public string GitCommit => ThisAssembly.Git.Commit;

        private IEnumerable<User> _Developers;

        /// <summary>
        /// Gets the list of lead developers to the Legere repository
        /// </summary>
        public IEnumerable<User> Developers
        {
            get => _Developers;
            private set => Set(ref _Developers, value);
        }

        private IEnumerable<object> _DonationMockupSource;

        /// <summary>
        /// Gets the mockup list to load the donation placeholder
        /// </summary>
        public IEnumerable<object> DonationMockupSource
        {
            get => _DonationMockupSource;
            private set => Set(ref _DonationMockupSource, value);
        }

        /// <summary>
        /// Loads all the necessary data for the view model
        /// </summary>
        public async Task LoadDataAsync()
        {
            Developers = new[] { await SimpleIoc.Default.GetInstance<IGitHubService>().GetUserAsync("Sergio0694") };
            DonationMockupSource = new[] { new object() };
        }
    }
}

