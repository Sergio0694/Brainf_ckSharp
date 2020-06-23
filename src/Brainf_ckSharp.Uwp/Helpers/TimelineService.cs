using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.UserActivities;
using Windows.Storage;
using Windows.UI.Shell;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Models.Ide;
using Microsoft.Toolkit.HighPerformance.Helpers;
using Nito.AsyncEx;
using Stubble.Core;

#nullable enable

namespace Brainf_ckSharp.Uwp.Helpers
{
    /// <summary>
    /// A <see langword="class"/> that saves and manages user activities for the app
    /// </summary>
    public sealed class TimelineService : IFilesHistoryService
    {
        /// <summary>
        /// The <see cref="Uri"/> for the template file
        /// </summary>
        private static readonly Uri TemplateUri = new Uri("ms-appx:///Assets/Misc/SourceCodeAdaptiveCard.mustache");

        /// <summary>
        /// Gets the collection of background images to use for the adaptive cards
        /// </summary>
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
        /// The synchronization mutex to create and manage user activities
        /// </summary>
        private readonly AsyncLock TimelineMutex = new AsyncLock();

        /// <summary>
        /// The adaptive card template, if loaded
        /// </summary>
        private static string? _Template;

        /// <summary>
        /// The current <see cref="UserActivitySession"/> in use, if any
        /// </summary>
        private UserActivitySession? _Session;

        /// <inheritdoc/>
        public async Task LogOrUpdateActivityAsync(IFile file)
        {
            using (await TimelineMutex.LockAsync())
            {
                // Load the template, if needed
                if (_Template is null)
                {
                    StorageFile templateFile = await StorageFile.GetFileFromApplicationUriAsync(TemplateUri);

                    _Template = await FileIO.ReadTextAsync(templateFile);
                }

                // Get a unique id for the file
                uint numericId = (uint)HashCode<char>.Combine(file.Path.AsSpan());
                string
                    textId = numericId.ToString(),
                    preview = await CodeLibraryEntry.LoadCodePreviewAsync(file),
                    background = BackgroundImages[(int)(numericId % BackgroundImages.Count)];

                // Create the model to represent the current activity
                AdaptiveCard model = new AdaptiveCard(file.DisplayName, preview, background);

                // Render the adaptive card
                string adaptiveCard = await StaticStubbleRenderer.Instance.RenderAsync(_Template, model);

                // Get the default channel and create the activity
                UserActivity activity = await UserActivityChannel.GetDefault().GetOrCreateUserActivityAsync(textId);

                // Set the deep-link and the title
                activity.ActivationUri = new Uri($"brainf-ck:///file?path={file.Path}");
                activity.VisualElements.DisplayText = file.DisplayName;
                activity.VisualElements.Content = AdaptiveCardBuilder.CreateAdaptiveCardFromJson(adaptiveCard);
                activity.IsRoamable = false;

                // Save to activity feed.
                await activity.SaveAsync();

                // Update the activity currently in use
                _Session?.Dispose();
                _Session = activity.CreateSession();
            }
        }

        /// <inheritdoc/>
        public async Task DismissCurrentActivityAsync()
        {
            using (await TimelineMutex.LockAsync())
            {
                _Session?.Dispose();
                _Session = null;
            }
        }

        /// <inheritdoc/>
        public async Task RemoveActivityAsync(IFile file)
        {
            using (await TimelineMutex.LockAsync())
            {
                // Get a unique id for the file
                string id = ((uint)HashCode<char>.Combine(file.Path.AsSpan())).ToString();

                // Remove the target activity
                await UserActivityChannel.GetDefault().DeleteActivityAsync(id);
            }
        }

        /// <summary>
        /// A model for an adaptive card for a recent file
        /// </summary>
        private sealed class AdaptiveCard
        {
            /// <summary>
            /// Creates a new <see cref="AdaptiveCard"/> instance with the specified parameters
            /// </summary>
            /// <param name="title">The title of the card</param>
            /// <param name="preview">The preview text to display</param>
            /// <param name="background">The URL to the background image</param>
            public AdaptiveCard(string title, string preview, string background)
            {
                Title = title;
                Preview = preview;
                Background = background;
            }

            /// <summary>
            /// Gets the title of the card
            /// </summary>
            public string Title { get; }

            /// <summary>
            /// Gets the preview text to display
            /// </summary>
            public string Preview { get; }

            /// <summary>
            /// Gets the URL to the background image
            /// </summary>
            public string Background { get; }
        }
    }
}
