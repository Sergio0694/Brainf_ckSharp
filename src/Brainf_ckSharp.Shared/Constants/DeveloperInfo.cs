namespace Brainf_ckSharp.Shared.Constants
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
}
