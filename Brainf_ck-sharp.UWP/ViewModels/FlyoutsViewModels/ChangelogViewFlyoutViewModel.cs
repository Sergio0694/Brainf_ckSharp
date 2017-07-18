using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Brainf_ck_sharp_UWP.DataModels;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using Brainf_ck_sharp_UWP.ViewModels.Abstract;

namespace Brainf_ck_sharp_UWP.ViewModels.FlyoutsViewModels
{
    public class ChangelogViewFlyoutViewModel : JumpListViewModelBase<ChangelogReleaseInfo, IReadOnlyList<string>>
    {
        // Private synchronization semaphore for the singleton changelog list
        private static readonly SemaphoreSlim ChangelogSemaphore = new SemaphoreSlim(1);

        // Singleton instance of the changelog entries collection
        private static IList<JumpListGroup<ChangelogReleaseInfo, IReadOnlyList<String>>> _Changelog;

        protected override async Task<IList<JumpListGroup<ChangelogReleaseInfo, IReadOnlyList<String>>>> OnLoadGroupsAsync()
        {
            await ChangelogSemaphore.WaitAsync();
            if (_Changelog == null) _Changelog = await Task.Run(() => GetChangelogData());
            ChangelogSemaphore.Release();
            return _Changelog;
        }

        // Builds the changelog items collection to show to the user
        private static IList<JumpListGroup<ChangelogReleaseInfo, IReadOnlyList<String>>> GetChangelogData()
        {
            // Create the output collection
            return new List<JumpListGroup<ChangelogReleaseInfo, IReadOnlyList<String>>>
            {
                CreateChangelogEntry("1.3.0.0", 2017, 7, 25, new List<String>
                {
                    "Added a settings panel with new IDE formatting options",
                    "Added an optional themes pack to customize the colors in the IDE",
                    "Added an option on Windows 10 Desktop to toggle between the classic and in-app acrylic brush effect",
                    "It is now possible to use the TAB key when writing text in the IDE",
                    "UI improvements when running the app on a high-DPI screen",
                    "Minor bug fixes and UI adjustments"
                }),
                CreateChangelogEntry("1.2.0.0", 2017, 7, 18, new List<String>
                {
                    "Added the byte overflow optional mode to both the console and the IDE",
                    "Added a one-time popup to ask for feedback on the app",
                    "Fixed a crash when using the IDE virtual arrows keyboard",
                    "The in-app notifications are now dismissed correctly when tapping the button in the top right corner",
                    "Added custom reveal-highlight effect to some UI elements (Fluent design)",
                    "Minor bug fixes and UI adjustments"
                }),
                CreateChangelogEntry("1.1.1.0", 2017, 7, 8, new List<String>
                {
                    "Added a button to delete the previous character in the IDE",
                    "It is no longer possible to try to run code with a syntax error from the IDE",
                    "Fixed an issue that was causing the save button to be disabled when navigating away and then back into the IDE",
                    "Minor UI tweaks and performance improvements"
                }),
                CreateChangelogEntry("1.1.0.0", 2017, 6, 30, new List<String>
                {
                    "Release notes section added to the info flyout",
                    "Added a missing description to the statistics section in the IDE result view",
                    "The compact memory viewer is now refreshed correctly when the console is restarted",
                    "Fixed a bug that was sometimes causing the code library not to be reflected when renaming a saved item",
                    "UI adjustments to the in-app flyouts on mobile phones and small screens",
                    "Minor fixes and improvements"
                }),
                CreateChangelogEntry("1.0.0.0", 2017, 6, 27, new List<String>
                {
                    "Initial release"
                })
            };
        }

        /// <summary>
        /// Creates a new group to display in the changelog view
        /// </summary>
        /// <param name="version">The release official version number</param>
        /// <param name="year">The release year</param>
        /// <param name="month">The release month</param>
        /// <param name="day">The release day</param>
        /// <param name="changes">A collection of changes in the current release</param>
        private static JumpListGroup<ChangelogReleaseInfo, IReadOnlyList<String>> CreateChangelogEntry(
            String version, int year, int month, int day, List<String> changes)
        {
            return new JumpListGroup<ChangelogReleaseInfo, IReadOnlyList<String>>(
                new ChangelogReleaseInfo(Version.Parse(version), new DateTime(year, month, day)), new List<List<String>> { changes });
        }
    }
}
