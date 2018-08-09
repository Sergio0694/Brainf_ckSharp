using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Brainf_ck_sharp_UWP.DataModels;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using Brainf_ck_sharp_UWP.ViewModels.Abstract.JumpList;

namespace Brainf_ck_sharp_UWP.ViewModels.FlyoutsViewModels
{
    public class ChangelogViewFlyoutViewModel : DeferredJumpListViewModelBase<ChangelogReleaseInfo, IReadOnlyList<string>>
    {
        // Private synchronization semaphore for the singleton changelog list
        private static readonly SemaphoreSlim ChangelogSemaphore = new SemaphoreSlim(1);

        // Singleton instance of the changelog entries collection
        private static IList<JumpListGroup<ChangelogReleaseInfo, IReadOnlyList<string>>> _Changelog;

        protected override async Task<IList<JumpListGroup<ChangelogReleaseInfo, IReadOnlyList<string>>>> OnLoadGroupsAsync()
        {
            await ChangelogSemaphore.WaitAsync();
            if (_Changelog == null) _Changelog = await Task.Run(() => GetChangelogData());
            ChangelogSemaphore.Release();
            return _Changelog;
        }

        // Builds the changelog items collection to show to the user
        private static IList<JumpListGroup<ChangelogReleaseInfo, IReadOnlyList<string>>> GetChangelogData()
        {
            // Create the output collection
            return new List<JumpListGroup<ChangelogReleaseInfo, IReadOnlyList<string>>>
            {
                CreateChangelogEntry("3.0.0.0", 2018, 8, 16, new List<string>
                {
                    "App rebuilt for Windows 10 April Update",
                    "Added support for TAB and SHIFT + TAB to indent the current selection in the IDE",
                    "Enabled export C code for scripts with PBrain operators",
                    "Added a debugging section to the user guide",
                    "UI improvements, more fluent design effects",
                    "Code tweaks and bug fixes"
                }),
                CreateChangelogEntry("2.0.1.0", 2017, 11, 10, new List<string>
                {
                    "The app splash screen is now optional",
                    "Minor UI tweaks to the light effects"
                }),
                CreateChangelogEntry("2.0.0.0", 2017, 10, 20, new List<string>
                {
                    "App rebuilt for Windows 10 Fall Creators Update",
                    "Minor UI adjustments and bug fixes"
                }),
                CreateChangelogEntry("1.4.1.0", 2017, 10, 4, new List<string>
                {
                    "Minor improvements and UI tweaks"
                }),
                CreateChangelogEntry("1.4.0.0", 2017, 9, 28, new List<string>
                {
                    "Added new support for PBrain extensions",
                    "Added a detailed user guide to the Brainf*ck language",
                    "IDE indentation guides rendering improved",
                    "Minor fixes and improvements"
                }),
                CreateChangelogEntry("1.3.1.0", 2017, 9, 7, new List<string>
                {
                    "Added an option to clear the Stdin buffer when executing a script from the Console or the IDE",
                    "Speed optimizations to the interpreter",
                    "Minor fixes and improvements"
                }),
                CreateChangelogEntry("1.3.0.0", 2017, 8, 26, new List<string>
                {
                    "Added a settings panel with new IDE formatting and UI options for PC and Mobile",
                    "Added an optional themes pack to customize the IDE",
                    "It is now possible to use the TAB key in the IDE",
                    "Added a small icon in the bottom bar to indicate if the current document has unsaved changes",
                    "Added a Ctrl + S keyboard shortcut to quickly save the code in the IDE",
                    "UI improvements when running on a high-DPI screen",
                    "Memory and performance improvements",
                    "Minor bug fixes and UI adjustments"
                }),
                CreateChangelogEntry("1.2.0.0", 2017, 7, 18, new List<string>
                {
                    "Added the byte overflow optional mode to both the console and the IDE",
                    "Added a one-time popup to ask for feedback on the app",
                    "Fixed a crash when using the IDE virtual arrows keyboard",
                    "The in-app notifications are now dismissed correctly when tapping the button in the top right corner",
                    "Added custom reveal-highlight effect to some UI elements (Fluent design)",
                    "Minor bug fixes and UI adjustments"
                }),
                CreateChangelogEntry("1.1.1.0", 2017, 7, 8, new List<string>
                {
                    "Added a button to delete a character in the IDE",
                    "It is no longer possible to try to run code with a syntax error from the IDE",
                    "Fixed an issue that was causing the save button to be disabled when navigating away and then back into the IDE",
                    "Minor UI tweaks and performance improvements"
                }),
                CreateChangelogEntry("1.1.0.0", 2017, 6, 30, new List<string>
                {
                    "Release notes section added to the info flyout",
                    "Added a missing description to the statistics section in the IDE result view",
                    "The compact memory viewer is now refreshed correctly when the console is restarted",
                    "Fixed a bug that was sometimes causing the code library not to be reflected when renaming a saved item",
                    "UI adjustments to the in-app flyouts on mobile phones and small screens",
                    "Minor fixes and improvements"
                }),
                CreateChangelogEntry("1.0.0.0", 2017, 6, 27, new List<string>
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
        private static JumpListGroup<ChangelogReleaseInfo, IReadOnlyList<string>> CreateChangelogEntry(
            string version, int year, int month, int day, List<string> changes)
        {
            return new JumpListGroup<ChangelogReleaseInfo, IReadOnlyList<string>>(
                new ChangelogReleaseInfo(Version.Parse(version), new DateTime(year, month, day)), new List<List<string>> { changes });
        }
    }
}
