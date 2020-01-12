using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Brainf_ckSharp.Uwp.Extensions.Windows.Storage;
using Brainf_ckSharp.Uwp.Messages.Ide;
using Brainf_ckSharp.Uwp.Models.Ide;
using Brainf_ckSharp.Uwp.ViewModels.Abstract.Collections;
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

        public async Task LoadAsync()
        {
            // Load the recent files
            IReadOnlyList<CodeLibraryEntry> recent = await Task.WhenAll(StorageApplicationPermissions.MostRecentlyUsedList.Entries.Select(async item =>
            {
                StorageFile file = await StorageApplicationPermissions.MostRecentlyUsedList.GetFileAsync(item.Token);
                CodeLibraryEntry? entry = await CodeLibraryEntry.TryLoadFromFileAsync(file);

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
    }
}
