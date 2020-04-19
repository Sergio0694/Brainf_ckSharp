using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.System;
using GitHub.APIs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.Helpers;
using User = GitHub.Models.User;

#nullable enable

namespace Legere.ViewModels.SubPages.Shell
{
    /// <summary>
    /// A view model for the about page in the app
    /// </summary>
    public sealed class AboutSubPageViewModel : ViewModelBase
    {
        /// <summary>
        /// Creates a new <see cref="AboutSubPageViewModel"/> instance
        /// </summary>
        public AboutSubPageViewModel()
        {
            LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        }

        /// <summary>
        /// Gets the <see cref="ICommand"/> instance responsible for loading the available source codes
        /// </summary>
        public ICommand LoadDataCommand { get; }

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

        private static IEnumerable<User>? _Developers;

        /// <summary>
        /// Gets the list of lead developers to the Legere repository
        /// </summary>
        public IEnumerable<User>? Developers
        {
            get => _Developers;
            private set => Set(ref _Developers, value);
        }

        private static IEnumerable<object>? _DonationMockupSource;

        /// <summary>
        /// Gets the mockup list to load the donation placeholder
        /// </summary>
        public IEnumerable<object>? DonationMockupSource
        {
            get => _DonationMockupSource;
            private set => Set(ref _DonationMockupSource, value);
        }

        /// <summary>
        /// Loads all the necessary data for the view model
        /// </summary>
        public async Task LoadDataAsync()
        {
            if (Developers != null) return;

            try
            {
                Developers = new[] { await ServiceProvider.GetRequiredService<IGitHubService>().GetUserAsync("Sergio0694") };
                DonationMockupSource = new[] { new object() };
            }
            catch
            {
                // Whoops!
            }
        }
    }
}

