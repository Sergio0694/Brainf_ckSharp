﻿namespace Brainf_ckSharp.Shared
{
    /// <summary>
    /// A <see langword="class"/> that exposes info about the app developer
    /// </summary>
    public static class DeveloperInfo
    {
        /// <summary>
        /// Developer feedback email
        /// </summary>
        public const string FeedbackEmail = "apps.sergiopedri@outlook.com";

        /// <summary>
        /// The URL to make donations on PayPal
        /// </summary>
        public const string GitHubUsername = "sergio0694";

        /// <summary>
        /// The URL to make donations on PayPal
        /// </summary>
        public const string PayPalMeUrl = "https://www.paypal.me/sergiopedri";

        /// <summary>
        /// A <see langword="class"/> with Store-related constants (eg. for IAPs)
        /// </summary>
        public static class Store
        {
            /// <summary>
            /// A <see langword="class"/> with data about the Store Ids for the available IAP
            /// </summary>
            public static class StoreIds
            {
                /// <summary>
                /// A <see langword="class"/> with data about the available IAPs for the app
                /// </summary>
                public static class IAPs
                {
                    /// <summary>
                    /// The Store Id of the IAP to unlock the themes
                    /// </summary>
                    public const string UnlockThemes = "9P4Q63CCFPBM";
                }
            }
        }
    }

    /// <summary>
    /// A <see langword="class"/> with the collection of tracked events for analytics
    /// </summary>
    public static class Events
    {
        public const string OnFileActivated = "[STARTUP] OnFileActivated";
        public const string OnActivated = "[STARTUP] OnActivated";
        public const string UserGuideOpened = "[SHELL] User guide opened";
        public const string UnicodeCharactersMapOpened = "[SHELL] Unicode characters map opened";
        public const string SettingsOpened = "[SHELL] Settings opened";
        public const string AboutPageOpened = "[SHELL] About page opened";
        public const string RepeatLastScript = "[CONSOLE] Repeat last script";
        public const string Restart = "[CONSOLE] Restart";
        public const string ConsoleRun = "[CONSOLE] Run";
        public const string ClearScreen = "[CONSOLE] Clear screen";
        public const string CompactMemoryViewerOpened = "[CONSOLE] Compact memory viewer opened";
        public const string NewFile = "[IDE] New file";
        public const string OpenCodeLibrary = "[IDE] Open code library";
        public const string OpenFile = "[IDE] Open file";
        public const string SaveAs = "[IDE] Save as";
        public const string Save = "[IDE] Save";
        public const string IdeRun = "[IDE] Run";
        public const string IdeDebug = "[IDE] Debug";
        public const string PickFileRequest = "[IDE] Pick file request";
        public const string LoadPickedFile = "[IDE] Load picked file";
        public const string SwitchToFile = "[IDE] Switch to file";
        public const string LoadLibrarySourceCode = "[IDE] Load library source code";
        public const string InsertCodeSnippet = "[IDE] Insert code snippet";
        public const string BreakpointAdded = "[IDE] Breakpoint added";
        public const string BreakpointRemoved = "[IDE] Breakpoint removed";
        public const string BreakpointsCleared = "[IDE] Breakpoints cleared";
        public const string ShareSourceCode = "[LIBRARY] Share source code";
        public const string CopySourceCode = "[LIBRARY] Copy source code";
        public const string ToggleFavoriteSourceCode = "[LIBRARY] Toggle favorite";
        public const string RemoveFromLibrary = "[LIBRARY] Remove source code";
        public const string DeleteSourceCode = "[LIBRARY] Delete source code";
        public const string SampleCodeSelected = "[LIBRARY] Sample code selected";
        public const string ThemesUnlockRequest = "[SETTINGS] Themes unlock request";
        public const string ThemeChanged = "[SETTINGS] Theme changed";
        public const string GitHubProfileOpened = "[ABOUT] GitHub profile opened";
        public const string PayPalDonationOpened = "[ABOUT] PayPal donation opened";
    }
}
