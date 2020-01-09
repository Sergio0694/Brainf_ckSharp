using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Brainf_ckSharp.Uwp.Messages.Ide;
using Brainf_ckSharp.Uwp.Models.Ide;
using Brainf_ckSharp.Uwp.ViewModels.Abstract.Collections;
using GalaSoft.MvvmLight.Messaging;

#nullable enable

namespace Brainf_ckSharp.Uwp.ViewModels.Controls.SubPages
{
    public sealed class CodeLibrarySubPageViewModel : GroupedItemsCollectionViewModelBase<string, CodeLibraryEntry>
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
            // Load the code samples
            IReadOnlyList<CodeLibraryEntry> samples = await GetSampleCodesAsync();

            Source.Add(new ObservableGroup<string, CodeLibraryEntry>("Sample files", samples));
        }

        /// <summary>
        /// Sends a request to load a specified code entry
        /// </summary>
        /// <param name="entry">The selected <see cref="CodeLibraryEntry"/> model</param>
        public async Task OpenFileAsync(CodeLibraryEntry entry)
        {
            if (SampleFilesMapping.Any(sample => Path.Combine(SampleFilesPath, $"{sample.Filename}.txt").Equals(entry.File.Path)))
            {
                Messenger.Default.Send(new LoadSourceCodeRequestMessage(await SourceCode.LoadFromReferenceFileAsync(entry.File)));
            }
            else if (await SourceCode.TryLoadFromEditableFileAsync(entry.File) is SourceCode sourceCode)
            {
                Messenger.Default.Send(new LoadSourceCodeRequestMessage(sourceCode));
            }
        }
    }
}
