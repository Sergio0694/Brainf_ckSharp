using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.UserActivities;
using Windows.Storage;
using Windows.UI.Shell;
using Brainf_ck_sharp_UWP.DataModels.SQLite;
using Brainf_ck_sharp_UWP.DataModels.SQLite.Enums;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Messages.IDE;
using Brainf_ckSharp.Legacy;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Helpers.WindowsAPIs
{
    /// <summary>
    /// A <see langword="class"/> that saves and manages user activities for the app
    /// </summary>
    public sealed class TimelineManager
    {
        #region Public APIs

        // The active instance, if present
        [CanBeNull]
        private static TimelineManager _Instance;

        private static bool _IsEnabled;

        /// <summary>
        /// Gets or sets whether or not the <see cref="TimelineManager"/> instance is currently logging user activities
        /// </summary>
        public static bool IsEnabled
        {
            get => _IsEnabled;
            set
            {
                if (_IsEnabled != value)
                {
                    if (_Instance != null) Messenger.Default.Unregister(_Instance);
                    if (!value) UserActivityChannel.GetDefault().DeleteAllActivitiesAsync().AsTask().Forget();
                    _Instance = value ? new TimelineManager() : null;
                    _IsEnabled = value;
                }
            }
        }

        #endregion

        #region Implementation

        // The synchronization mutex to create and manage user activities
        [NotNull]
        private readonly AsyncMutex TimelineMutex = new AsyncMutex();

        // The current activity in use
        [CanBeNull]
        private UserActivitySession _Session;

        // Builds a new instance and subscribes to the required messages
        private TimelineManager()
        {
            Messenger.Default.Register<WorkingSourceCodeChangedMessage>(this, m =>
            {
                if (m.Value == null) _Session?.Dispose();
                else if (m.Value.Type != SavedSourceCodeType.Sample) LogUserSessionAsync(m.Value.Code).Forget();
            });
        }

        /// <summary>
        /// Gets the collection of background images to use for the adaptive cards
        /// </summary>
        [NotNull, ItemNotNull]
        private static readonly IReadOnlyList<string> BackgroundImages = new[]
        {
            "https://i.imgur.com/PBramWx.png",
            "https://i.imgur.com/Ibcyqz2.png",
            "https://i.imgur.com/s2ATSPZ.png",
            "https://i.imgur.com/6FaipLq.png",
            "https://i.imgur.com/886BaOq.png",
            "https://i.imgur.com/M01vWBf.png",
            "https://i.imgur.com/LEtGjHa.png",
            "https://i.imgur.com/V5sw76e.png",
            "https://i.imgur.com/ElimxNR.png",
            "https://i.imgur.com/RB0bOD5.png"
        };

        /// <summary>
        /// Logs a new activity, or updates the existing one, for the input saved code
        /// </summary>
        /// <param name="code">The <see cref="SourceCode"/> instance the user is currently working on</param>
        private async Task LogUserSessionAsync([NotNull] SourceCode code)
        {
            // Lock
            using (await TimelineMutex.LockAsync())
            {
                // Get the default channel and create the activity
                UserActivityChannel channel = UserActivityChannel.GetDefault();
                UserActivity activity = await channel.GetOrCreateUserActivityAsync(code.Uid);

                // Set the deep-link and the title
                activity.ActivationUri = new Uri($"brainf-ck://ide?id={code.Uid}");
                activity.VisualElements.DisplayText = "title";

                // Get the image URL
                string url;
                using (MD5 md5 = MD5.Create())
                {
                    byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(code.Uid));
                    int ones = hash.Sum(b => Convert.ToString(b, 2).ToCharArray().Count(c => c == '1'));
                    url = BackgroundImages[ones % BackgroundImages.Count];
                }

                // Create the adaptive card
                StorageFile cardFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/AdaptiveCards/SavedCodeCard.json"));
                string cardText = (await FileIO.ReadTextAsync(cardFile))
                    .Replace("{BACKGROUND}", url)
                    .Replace("{TITLE}", code.Title)
                    .Replace("{PREVIEW}", new string(code.Code.Where(c => Brainf_ckInterpreter.Operators.Contains(c)).ToArray()));
                activity.VisualElements.Content = AdaptiveCardBuilder.CreateAdaptiveCardFromJson(cardText);

                // Save to activity feed.
                await activity.SaveAsync();
                _Session = activity.CreateSession();
            }
        }

        #endregion
    }
}
