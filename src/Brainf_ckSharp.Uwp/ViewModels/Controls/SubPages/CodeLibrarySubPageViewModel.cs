using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Brainf_ckSharp.Uwp.Enums;
using Brainf_ckSharp.Uwp.Messages.Ide;
using Brainf_ckSharp.Uwp.Models.Ide;
using Brainf_ckSharp.Uwp.Services.Clipboard;
using Brainf_ckSharp.Uwp.Services.Share;
using Brainf_ckSharp.Uwp.ViewModels.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Extensions;
using Microsoft.Toolkit.HighPerformance.Extensions;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;

#nullable enable

namespace Brainf_ckSharp.Uwp.ViewModels.Controls.SubPages
{
    public sealed class CodeLibrarySubPageViewModel : GroupedItemsCollectionViewModelBase<CodeLibrarySection, object>
    {
        /// <summary>
        /// The path of folder that contains the sample files
        /// </summary>
        private static readonly string SampleFilesPath = $@"{Package.Current.InstalledLocation.Path}\Assets\Samples\";

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
        private static async ValueTask<IReadOnlyList<CodeLibraryEntry>> GetSampleCodesAsync()
        {
            return _SampleCodes ??= await Task.WhenAll(SampleFilesMapping.Select(async item =>
            {
                string path = Path.Combine(SampleFilesPath, $"{item.Filename}.txt");
                StorageFile file = await StorageFile.GetFileFromPathAsync(path);
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
            RemoveFromLibraryCommand = new RelayCommand<CodeLibraryEntry>(RemoveFromLibrary);
            DeleteCommand = new RelayCommand<CodeLibraryEntry>(Delete);
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
        /// Loads the currently available code samples and recently used files
        /// </summary>
        public async Task LoadDataAsync()
        {
            // Load the recent files
            IReadOnlyList<AccessListEntry> entries = StorageApplicationPermissions.MostRecentlyUsedList.Entries.ToArray();
            IReadOnlyList<CodeLibraryEntry?> recent = await Task.WhenAll(entries.Select(async item =>
            {
                // Try to get the target file
                StorageFile? file = await StorageApplicationPermissions.MostRecentlyUsedList.TryGetFileAsync(item.Token);
                if (file is null)
                {
                    StorageApplicationPermissions.MostRecentlyUsedList.Remove(item.Token);
                    return null;
                }

                // Deserialize the metadata and prepare the model
                CodeMetadata metadata = string.IsNullOrEmpty(item.Metadata) ? new CodeMetadata() : JsonSerializer.Deserialize<CodeMetadata>(item.Metadata);
                CodeLibraryEntry? entry = await CodeLibraryEntry.TryLoadFromFileAsync(file, metadata);

                return entry ?? throw new InvalidOperationException($"Failed to load token {item.Token}");
            }));

            // Sort chronologically
            IReadOnlyList<CodeLibraryEntry?> sorted = recent.OrderByDescending(entry => entry?.EditTime).ToArray();

            // Load the code samples
            IReadOnlyList<CodeLibraryEntry> samples = await GetSampleCodesAsync();

            // Add the favorites, if any
            IEnumerable<CodeLibraryEntry> favorited = sorted.Where(entry => entry?.Metadata.IsFavorited == true)!;
            Source.Add(CodeLibrarySection.Favorites, favorited.Append<object>(CodeLibrarySection.Favorites));

            // Add the recent and sample items
            IEnumerable<CodeLibraryEntry> unfavorited = sorted.Where(entry => entry?.Metadata.IsFavorited == false)!;
            Source.Add(CodeLibrarySection.Recent, unfavorited.Append<object>(CodeLibrarySection.Recent));
            Source.Add(CodeLibrarySection.Samples, samples);
        }

        /// <summary>
        /// Processes a given item
        /// </summary>
        /// <param name="item">The target item to process</param>
        public void ProcessItem(object item)
        {
            if (item is CodeLibraryEntry entry) _ = OpenFileAsync(entry);
            else if (item is CodeLibrarySection c) RequestOpenFile(c == CodeLibrarySection.Favorites);
            else throw new ArgumentException("The input item can't be null");
        }

        /// <summary>
        /// Requests to pick and open a source code file
        /// </summary>
        public void RequestOpenFile(bool favorite) => Messenger.Send(new PickOpenFileRequestMessage(favorite));

        /// <summary>
        /// Sends a request to load a specified code entry
        /// </summary>
        /// <param name="entry">The selected <see cref="CodeLibraryEntry"/> model</param>
        public async Task OpenFileAsync(CodeLibraryEntry entry)
        {
            if (entry.File.IsFromPackageDirectory())
            {
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
        public void ToggleFavorite(CodeLibraryEntry entry)
        {
            // Shared comparison function to insert items in a target group
            static int Comparer(object a, object b)
            {
                if (a is CodeLibraryEntry left && b is CodeLibraryEntry right) return left.EditTime.CompareTo(right.EditTime);
                return -1;
            }

            /* If the current item is favorited, set is as not favorited
             * and move it back into the recent files section.
             * If the favorites section becomes empty, remove it entirely. */
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

            StorageApplicationPermissions.MostRecentlyUsedList.AddOrReplace(entry.File.GetId(), entry.File, JsonSerializer.Serialize(entry.Metadata));
        }

        /// <summary>
        /// Copies the content of a specified entry to the clipboard
        /// </summary>
        /// <param name="entry">The <see cref="CodeLibraryEntry"/> instance to copy to the clipboard</param>
        public async Task CopyToClipboardAsync(CodeLibraryEntry entry)
        {
            string text = await FileIO.ReadTextAsync(entry.File);

            Ioc.Default.GetRequiredService<IClipboardService>().TryCopy(text);
        }

        /// <summary>
        /// Shares a specified entry
        /// </summary>
        /// <param name="entry">The <see cref="CodeLibraryEntry"/> instance to share</param>
        public void Share(CodeLibraryEntry entry)
        {
            Ioc.Default.GetRequiredService<IShareService>().Share(entry.Title, entry.File);
        }

        /// <summary>
        /// Removes a specific <see cref="CodeLibraryEntry"/> instance from the code library
        /// </summary>
        /// <param name="entry">The <see cref="CodeLibraryEntry"/> instance to remove</param>
        public void RemoveFromLibrary(CodeLibraryEntry entry)
        {
            var group = Source.First(g => g.Contains(entry));

            if (group.Count == 1) Source.Remove(group);
            else group.Remove(entry);

            StorageApplicationPermissions.MostRecentlyUsedList.Remove(entry.File.Path.GetDjb2HashCode().ToHexString());
        }

        /// <summary>
        /// Deletes a specific <see cref="CodeLibraryEntry"/> instance in the code library
        /// </summary>
        /// <param name="entry">The <see cref="CodeLibraryEntry"/> instance to delete</param>
        public void Delete(CodeLibraryEntry entry)
        {
            RemoveFromLibrary(entry);

            _ = entry.File.DeleteAsync();
        }
    }
}
