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
using Brainf_ckSharp.Uwp.Extensions.Windows.Storage;
using Brainf_ckSharp.Uwp.Messages.Ide;
using Brainf_ckSharp.Uwp.Models.Ide;
using Brainf_ckSharp.Uwp.ViewModels.Abstract.Collections;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

#nullable enable

namespace Brainf_ckSharp.Uwp.ViewModels.Controls.SubPages
{
    public sealed class CodeLibrarySubPageViewModel : GroupedItemsCollectionViewModelBase<CodeLibraryCategory, object>
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
            LoadDataCommand = new RelayCommand(() => _ = LoadAsync());
            RequestOpenFileCommand = new RelayCommand(RequestOpenFile);
            OpenFileCommand = new RelayCommand<CodeLibraryEntry>(m => _ = OpenFileAsync(m));
            ToggleFavoriteCommand = new RelayCommand<CodeLibraryEntry>(ToggleFavorite);
        }

        /// <summary>
        /// Gets the <see cref="ICommand"/> instance responsible for loading the available source codes
        /// </summary>
        public ICommand LoadDataCommand { get; }

        /// <summary>
        /// Gets the <see cref="ICommand"/> instance responsible for picking a file to open
        /// </summary>
        public ICommand RequestOpenFileCommand { get; }

        /// <summary>
        /// Gets the <see cref="ICommand"/> instance responsible for opening a file
        /// </summary>
        public ICommand OpenFileCommand { get; }

        /// <summary>
        /// Gets the <see cref="ICommand"/> instance responsible for toggling a favorite item
        /// </summary>
        public ICommand ToggleFavoriteCommand { get; }

        /// <summary>
        /// Loads the currently available code samples and recently used files
        /// </summary>
        public async Task LoadAsync()
        {
            // Load the recent files
            IReadOnlyList<CodeLibraryEntry> recent = await Task.WhenAll(StorageApplicationPermissions.MostRecentlyUsedList.Entries.Select(async item =>
            {
                StorageFile file = await StorageApplicationPermissions.MostRecentlyUsedList.GetFileAsync(item.Token);
                CodeMetadata metadata = string.IsNullOrEmpty(item.Metadata) ? new CodeMetadata() : JsonSerializer.Deserialize<CodeMetadata>(item.Metadata);
                CodeLibraryEntry? entry = await CodeLibraryEntry.TryLoadFromFileAsync(file, metadata);

                return entry ?? throw new InvalidOperationException($"Failed to load token {item.Token}");
            }));

            // Load the code samples
            IReadOnlyList<CodeLibraryEntry> samples = await GetSampleCodesAsync();

            Source.Add(new ObservableGroup<CodeLibraryCategory, object>(CodeLibraryCategory.Recent, recent.Append(new object())));
            Source.Add(new ObservableGroup<CodeLibraryCategory, object>(CodeLibraryCategory.Samples, samples));
        }

        /// <summary>
        /// Requests to pick and open a source code file
        /// </summary>
        public void RequestOpenFile() => Messenger.Default.Send<PickOpenFileRequestMessage>();

        /// <summary>
        /// Sends a request to load a specified code entry
        /// </summary>
        /// <param name="entry">The selected <see cref="CodeLibraryEntry"/> model</param>
        public async Task OpenFileAsync(CodeLibraryEntry entry)
        {
            if (entry.File.IsFromPackageDirectory())
            {
                SourceCode code = await SourceCode.LoadFromReferenceFileAsync(entry.File);
                Messenger.Default.Send(new LoadSourceCodeRequestMessage(code));
            }
            else
            {
                if (!(await SourceCode.TryLoadFromEditableFileAsync(entry.File) is SourceCode sourceCode)) return;
                Messenger.Default.Send(new LoadSourceCodeRequestMessage(sourceCode));
            }
        }

        /// <summary>
        /// Toggles the favorite state of a given <see cref="CodeLibraryEntry"/> instance
        /// </summary>
        /// <param name="entry">The <see cref="CodeLibraryEntry"/> instance to toggle</param>
        public void ToggleFavorite(CodeLibraryEntry entry)
        {
            /* If the current item is favorited, set is as not favorited
             * and move it back into the recent files section.
             * If the favorites section becomes empty, remove it entirely. */
            if (entry.Metadata.IsFavorited)
            {
                entry.Metadata.IsFavorited = false;

                if (Source[0].Count == 1) Source.RemoveAt(0);
                else Source[0].Remove(entry);

                var group = Source.First(g => g.Key == CodeLibraryCategory.Recent);
                group.Insert(0, entry);
            }
            else
            {
                entry.Metadata.IsFavorited = true;

                Source.First(g => g.Key == CodeLibraryCategory.Recent).Remove(entry);

                if (Source[0].Key == CodeLibraryCategory.Favorites) Source[0].Insert(0, entry);
                else Source.Insert(0, new ObservableGroup<CodeLibraryCategory, object>(CodeLibraryCategory.Favorites, entry.AsEnumerable()));
            }
        }
    }
}
