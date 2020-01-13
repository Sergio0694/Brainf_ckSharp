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
using Brainf_ckSharp.Uwp.Extensions.Windows.Storage;
using Brainf_ckSharp.Uwp.Messages.Ide;
using Brainf_ckSharp.Uwp.Models.Ide;
using Brainf_ckSharp.Uwp.ViewModels.Abstract.Collections;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

#nullable enable

namespace Brainf_ckSharp.Uwp.ViewModels.Controls.SubPages
{
    public sealed class CodeLibrarySubPageViewModel : GroupedItemsCollectionViewModelBase<string, object>
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
            ToggleFavoriteCommand = new RelayCommand<CodeLibraryEntry>(ToggleFavorite);
        }

        /// <summary>
        /// Gets the <see cref="ICommand"/> instance responsible for loading the available source codes
        /// </summary>
        public ICommand LoadDataCommand { get; }

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

            Source.Add(new ObservableGroup<string, object>("Recent files", recent.Append(new object())));
            Source.Add(new ObservableGroup<string, object>("Sample files", samples));
        }

        /// <summary>
        /// Sends a request to load a specified code entry
        /// </summary>
        /// <param name="model">The selected <see cref="CodeLibraryEntry"/> model, if present</param>
        public async Task OpenFileAsync(object model)
        {
            switch (model)
            {
                case CodeLibraryEntry sample when sample.File.IsFromPackageDirectory():
                    Messenger.Default.Send(new LoadSourceCodeRequestMessage(await SourceCode.LoadFromReferenceFileAsync(sample.File)));
                    break;
                case CodeLibraryEntry entry:
                    if (!(await SourceCode.TryLoadFromEditableFileAsync(entry.File) is SourceCode sourceCode)) return;
                    Messenger.Default.Send(new LoadSourceCodeRequestMessage(sourceCode));
                    break;
                case object _:
                    Messenger.Default.Send<PickOpenFileRequestMessage>();
                    break;
                default: throw new ArgumentException("The input model can't be null");
            }
        }

        /// <summary>
        /// Toggles the favorite state of a given <see cref="CodeLibraryEntry"/> instance
        /// </summary>
        /// <param name="entry">The <see cref="CodeLibraryEntry"/> instance to toggle</param>
        public void ToggleFavorite(CodeLibraryEntry entry)
        {
            if (entry.Metadata.IsFavorited)
            {
                entry.Metadata.IsFavorited = false;

                if (Source[0].Count == 1) Source.RemoveAt(0);
                else Source[0].Remove(entry);

                var group = Source.First(g => g.Key == "Recent files");

                group.Insert(0, entry);
            }
            else
            {
                entry.Metadata.IsFavorited = true;

                Source.First(g => g.Key == "Recent files").Remove(entry);

                if (Source[0].Key == "Favorites") Source[0].Insert(0, entry);
                else Source.Insert(0, new ObservableGroup<string, object>("Favorites", new[] {entry }));
            }
        }
    }
}
