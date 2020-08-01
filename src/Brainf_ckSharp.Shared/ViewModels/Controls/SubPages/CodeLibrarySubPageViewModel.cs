using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Enums;
using Brainf_ckSharp.Shared.Extensions.Microsoft.Toolkit.Collections;
using Brainf_ckSharp.Shared.Extensions.System.Collections.Generic;
using Brainf_ckSharp.Shared.Messages.Ide;
using Brainf_ckSharp.Shared.Models.Ide;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;

#nullable enable

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages
{
    public sealed class CodeLibrarySubPageViewModel : ObservableRecipient
    {
        /// <summary>
        /// The <see cref="IAnalyticsService"/> instance currently in use
        /// </summary>
        private readonly IAnalyticsService AnalyticsService = Ioc.Default.GetRequiredService<IAnalyticsService>();

        /// <summary>
        /// The <see cref="IFilesService"/> instance currently in use
        /// </summary>
        private readonly IFilesService FilesService = Ioc.Default.GetRequiredService<IFilesService>();

        /// <summary>
        /// The <see cref="IFilesHistoryService"/> instance currently in use
        /// </summary>
        private readonly IFilesHistoryService FilesHistoryService = Ioc.Default.GetRequiredService<IFilesHistoryService>();

        /// <summary>
        /// The <see cref="IClipboardService"/> instance currently in use
        /// </summary>
        private readonly IClipboardService ClipboardService = Ioc.Default.GetRequiredService<IClipboardService>();

        /// <summary>
        /// The <see cref="IShareService"/> instance currently in use
        /// </summary>
        private readonly IShareService ShareService = Ioc.Default.GetRequiredService<IShareService>();

        /// <summary>
        /// The relative path of folder that contains the sample files
        /// </summary>
        private static readonly string SampleFilesRelativePath = @"Assets\Samples\";

        /// <summary>
        /// The ordered mapping of available source code files
        /// </summary>
        private static readonly IReadOnlyList<(string Title, string Filename)> SampleFilesMapping = new[]
        {
            ("Hello World!", "HelloWorld"),
            ("Unicode value", "UnicodeValue"),
            ("Unicode sum", "UnicodeSum"),
            ("Integer sum", "IntegerSum"),
            ("Integer division", "IntegerDivision"),
            ("Fibonacci", "Fibonacci"),
            ("Header comment", "HeaderComment")
        };

        /// <summary>
        /// The cached collection of sample codes, if available
        /// </summary>
        private static IReadOnlyList<CodeLibraryEntry>? _SampleCodes;

        /// <summary>
        /// Loads the available code samples
        /// </summary>
        /// <returns>A <see cref="IReadOnlyList{T}"/> instance with the loaded code samples</returns>
        [Pure]
        private async ValueTask<IReadOnlyList<CodeLibraryEntry>> GetSampleCodesAsync()
        {
            return _SampleCodes ??= await Task.WhenAll(SampleFilesMapping.Select(async item =>
            {
                string installationPath = FilesService.InstallationPath;
                string path = Path.Combine(installationPath, SampleFilesRelativePath, $"{item.Filename}.txt");
                IFile file = await FilesService.GetFileFromPathAsync(path);

                CodeLibraryEntry? entry = await CodeLibraryEntry.TryLoadFromFileAsync(file, item.Title);

                return entry ?? throw new InvalidOperationException($"Failed to load {item.Title} sample");
            }));
        }

        /// <summary>
        /// Creates a new <see cref="CodeLibrarySubPageViewModel"/> instance
        /// </summary>
        public CodeLibrarySubPageViewModel()
        {
            LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
            ProcessItemCommand = new RelayCommand<object>(ProcessItem);
            ToggleFavoriteCommand = new RelayCommand<CodeLibraryEntry>(ToggleFavorite);
            CopyToClipboardCommand = new AsyncRelayCommand<CodeLibraryEntry>(CopyToClipboardAsync);
            ShareCommand = new RelayCommand<CodeLibraryEntry>(Share);
            RemoveFromLibraryCommand = new AsyncRelayCommand<CodeLibraryEntry>(RemoveFromLibraryAsync);
            DeleteCommand = new AsyncRelayCommand<CodeLibraryEntry>(DeleteAsync);
        }

        /// <summary>
        /// Gets the <see cref="ICommand"/> instance responsible for loading the available source codes
        /// </summary>
        public ICommand LoadDataCommand { get; }

        /// <summary>
        /// Gets the <see cref="ICommand"/> instance responsible for processing a selected item
        /// </summary>
        public ICommand ProcessItemCommand { get; }

        /// <summary>
        /// Gets the <see cref="ICommand"/> instance responsible for toggling a favorite item
        /// </summary>
        public ICommand ToggleFavoriteCommand { get; }

        /// <summary>
        /// Gets the <see cref="ICommand"/> instance responsible for copying an item to the clipboard
        /// </summary>
        public ICommand CopyToClipboardCommand { get; }

        /// <summary>
        /// Gets the <see cref="ICommand"/> instance responsible for sharing an item
        /// </summary>
        public ICommand ShareCommand { get; }

        /// <summary>
        /// Gets the <see cref="ICommand"/> instance responsible for removing an item from the library
        /// </summary>
        public ICommand RemoveFromLibraryCommand { get; }

        /// <summary>
        /// Gets the <see cref="ICommand"/> instance responsible for deleting an item in the library
        /// </summary>
        public ICommand DeleteCommand { get; }

        /// <summary>
        /// Gets the current collection of sections to display
        /// </summary>
        public ObservableGroupedCollection<CodeLibrarySection, object> Source { get; } = new ObservableGroupedCollection<CodeLibrarySection, object>();

        /// <summary>
        /// Loads the currently available code samples and recently used files
        /// </summary>
        private async Task LoadDataAsync()
        {
            List<CodeLibraryEntry> recent = new List<CodeLibraryEntry>();

            // Load the recent files
            await foreach ((IFile file, string data) in FilesService.GetFutureAccessFilesAsync())
            {
                // Deserialize the metadata and prepare the model
                CodeMetadata metadata = string.IsNullOrEmpty(data) ? new CodeMetadata() : JsonSerializer.Deserialize<CodeMetadata>(data);
                CodeLibraryEntry entry = await CodeLibraryEntry.TryLoadFromFileAsync(file, metadata)
                                         ?? throw new InvalidOperationException("Failed to load source code");

                recent.Add(entry);
            }

            // Sort chronologically
            IReadOnlyList<CodeLibraryEntry> sorted = recent.OrderByDescending(entry => entry.EditTime).ToArray();

            // Load the code samples
            IReadOnlyList<CodeLibraryEntry> samples = await GetSampleCodesAsync();

            // Add the favorites, if any
            IEnumerable<CodeLibraryEntry> favorited = sorted.Where(entry => entry.Metadata.IsFavorited)!;
            Source.Add(CodeLibrarySection.Favorites, favorited.Append<object>(CodeLibrarySection.Favorites));

            // Add the recent and sample items
            IEnumerable<CodeLibraryEntry> unfavorited = sorted.Where(entry => !entry.Metadata.IsFavorited)!;
            Source.Add(CodeLibrarySection.Recent, unfavorited.Append<object>(CodeLibrarySection.Recent));
            Source.Add(CodeLibrarySection.Samples, samples);
        }

        /// <summary>
        /// Processes a given item
        /// </summary>
        /// <param name="item">The target item to process</param>
        private void ProcessItem(object item)
        {
            if (item is CodeLibraryEntry entry) _ = OpenFileAsync(entry);
            else if (item is CodeLibrarySection c) RequestOpenFile(c == CodeLibrarySection.Favorites);
            else throw new ArgumentException("The input item can't be null");
        }

        /// <summary>
        /// Requests to pick and open a source code file
        /// </summary>
        private void RequestOpenFile(bool favorite) => Messenger.Send(new PickOpenFileRequestMessage(favorite));

        /// <summary>
        /// Sends a request to load a specified code entry
        /// </summary>
        /// <param name="entry">The selected <see cref="CodeLibraryEntry"/> model</param>
        private async Task OpenFileAsync(CodeLibraryEntry entry)
        {
            if (entry.File.IsReadOnly)
            {
                AnalyticsService.Log(EventNames.SampleCodeSelected, (nameof(SourceCode.File), entry.File.DisplayName));

                SourceCode code = await SourceCode.LoadFromReferenceFileAsync(entry.File);

                Messenger.Send(new LoadSourceCodeRequestMessage(code));
            }
            else
            {
                if (!(await SourceCode.TryLoadFromEditableFileAsync(entry.File) is SourceCode sourceCode)) return;

                Messenger.Send(new LoadSourceCodeRequestMessage(sourceCode));
            }
        }

        /// <summary>
        /// Toggles the favorite state of a given <see cref="CodeLibraryEntry"/> instance
        /// </summary>
        /// <param name="entry">The <see cref="CodeLibraryEntry"/> instance to toggle</param>
        private void ToggleFavorite(CodeLibraryEntry entry)
        {
            AnalyticsService.Log(EventNames.ToggleFavoriteSourceCode, (nameof(CodeMetadata.IsFavorited), entry.Metadata.IsFavorited.ToString()));

            // Shared comparison function to insert items in a target group
            static int Comparer(object a, object b)
            {
                if (a is CodeLibraryEntry left && b is CodeLibraryEntry right) return left.EditTime.CompareTo(right.EditTime);
                return -1;
            }

            // If the current item is favorited, set is as not favorited
            // and move it back into the recent files section.
            // If the favorites section becomes empty, remove it entirely.
            if (entry.Metadata.IsFavorited)
            {
                entry.Metadata.IsFavorited = false;

                Source[0].Remove(entry);
                Source[1].InsertSorted(entry, Comparer);
            }
            else
            {
                entry.Metadata.IsFavorited = true;

                Source[1].Remove(entry);
                Source[0].InsertSorted(entry, Comparer);
            }

            entry.File.RequestFutureAccessPermission(JsonSerializer.Serialize(entry.Metadata));
        }

        /// <summary>
        /// Copies the content of a specified entry to the clipboard
        /// </summary>
        /// <param name="entry">The <see cref="CodeLibraryEntry"/> instance to copy to the clipboard</param>
        private async Task CopyToClipboardAsync(CodeLibraryEntry entry)
        {
            AnalyticsService.Log(EventNames.CopySourceCode);

            string text = await entry.File.ReadAllTextAsync();

            ClipboardService.TryCopy(text);
        }

        /// <summary>
        /// Shares a specified entry
        /// </summary>
        /// <param name="entry">The <see cref="CodeLibraryEntry"/> instance to share</param>
        private void Share(CodeLibraryEntry entry)
        {
            AnalyticsService.Log(EventNames.ShareSourceCode);

            ShareService.Share(entry.Title, entry.File);
        }

        /// <summary>
        /// Removes a source code from the library and removes the relative permissions
        /// </summary>
        /// <param name="entry">The <see cref="CodeLibraryEntry"/> instance to remove</param>
        private void RemoveTrackedSourceCode(CodeLibraryEntry entry)
        {
            var group = Source.First(g => g.Contains(entry));

            if (group.Count == 1) Source.Remove(group);
            else group.Remove(entry);

            entry.File.RemoveFutureAccessPermission();
        }

        /// <summary>
        /// Removes a specific <see cref="CodeLibraryEntry"/> instance from the code library
        /// </summary>
        /// <param name="entry">The <see cref="CodeLibraryEntry"/> instance to remove</param>
        private Task RemoveFromLibraryAsync(CodeLibraryEntry entry)
        {
            AnalyticsService.Log(EventNames.RemoveFromLibrary, (nameof(CodeMetadata.IsFavorited), entry.Metadata.IsFavorited.ToString()));

            RemoveTrackedSourceCode(entry);

            return FilesHistoryService.RemoveActivityAsync(entry.File);
        }

        /// <summary>
        /// Deletes a specific <see cref="CodeLibraryEntry"/> instance in the code library
        /// </summary>
        /// <param name="entry">The <see cref="CodeLibraryEntry"/> instance to delete</param>
        private Task DeleteAsync(CodeLibraryEntry entry)
        {
            AnalyticsService.Log(EventNames.DeleteSourceCode, (nameof(CodeMetadata.IsFavorited), entry.Metadata.IsFavorited.ToString()));

            RemoveTrackedSourceCode(entry);

            // We can remove the item from history and delete the file in parallel,
            // since removing a tracked item from history requires no file access.
            return Task.WhenAll(
                FilesHistoryService.RemoveActivityAsync(entry.File),
                entry.File.DeleteAsync());
        }
    }
}
