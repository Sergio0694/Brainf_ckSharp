namespace Brainf_ckSharp.Uwp.Constants
{
    /// <summary>
    /// A <see langword="class"/> that exposes some constants in use
    /// </summary>
    public static class Constants
    {
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
            /// The URL to download the app from the Store
            /// </summary>
            public static readonly string AppUrl = $"https://www.microsoft.com/store/apps/{StoreIds.AppId}";

            /// <summary>
            /// A <see langword="class"/> with data about the Store Ids for the available IAP
            /// </summary>
            public static class StoreIds
            {
                /// <summary>
                /// The Store Id of the app
                /// </summary>
                public const string AppId = "9NBLGGGZHVQ5";

                /// <summary>
                /// A <see langword="class"/> with data about the available IAPs for the app
                /// </summary>
                public static class IAPs
                {
                    /// <summary>
                    /// The Store Id of the IAP to unlock the themes
                    /// </summary>
                    public const string UnlockThemesIap = "9P4Q63CCFPBM";
                }
            }
        }
    }
}
