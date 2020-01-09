using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Brainf_ckSharp.Uwp.ViewModels.Abstract.Collections;

#nullable enable

namespace Brainf_ckSharp.Uwp.ViewModels.Controls.SubPages
{
    public sealed class CodeLibrarySubPageViewModel : GroupedItemsCollectionViewModelBase<string, string>
    {
        /// <summary>
        /// The desired length of each loaded code preview
        /// </summary>
        private const int CodePreviewLength = 120;

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

        private static IReadOnlyList<string>? _SampleCodes;

        /// <summary>
        /// Loads the available code samples
        /// </summary>
        /// <returns>A <see cref="IReadOnlyList{T}"/> instance with the loaded code samples</returns>
        [Pure]
        private static async ValueTask<IReadOnlyList<string>> GetSampleCodesAsync()
        {
            return _SampleCodes ??= await Task.WhenAll(SampleFilesMapping.Select(async item =>
            {
                string path = Path.Combine(SampleFilesPath, $"{item.Filename}.txt");
                StorageFile file = await StorageFile.GetFileFromPathAsync(path);
                string preview = await LoadCodePreviewAsync(file, CodePreviewLength);

                return preview;
            }));
        }

        public async Task LoadAsync()
        {
            // Load the code samples
            IReadOnlyList<string> samples = await GetSampleCodesAsync();

            Source.Add(new ObservableGroup<string, string>("Sample files", samples));
        }

        /// <summary>
        /// Loads a code preview with a maximum specified length from a given file
        /// </summary>
        /// <param name="file">The input <see cref="StorageFile"/> to read from</param>
        /// <param name="length">The maximum length of the preview to load</param>
        /// <returns>A <see cref="string"/> with a preview of the Brainf*ck/PBrain source code in <paramref name="file"/></returns>
        [Pure]
        public static async Task<string> LoadCodePreviewAsync(StorageFile file, int length)
        {
            Guard.MustBeGreaterThan(length, 0, nameof(length));

            // Open the input file and a streama reader to decode the text
            using Stream stream = await file.OpenStreamForReadAsync();
            using StreamReader reader = new StreamReader(stream);

            // Allocate a temporary buffer for the resulting characters and one for the blocks to read
            char[] charBuffer = ArrayPool<char>.Shared.Rent(length);
            char[] tempBuffer = ArrayPool<char>.Shared.Rent(length);

            int previewLength = 0;

            try
            {
                while (previewLength < length)
                {
                    // Read a new block of characters from the input stream
                    int maxCharactersToRead = length - previewLength;
                    int read = await reader.ReadAsync(tempBuffer, 0, maxCharactersToRead);

                    if (read == 0) break;

                    // Accumulate the operators in the current block
                    previewLength += ExtractOperators(new ReadOnlySpan<char>(tempBuffer, 0, read), charBuffer.AsSpan(previewLength));
                }

                // Create a string with the parsed operators up to this point
                return new string(charBuffer, 0, previewLength);
            }
            finally
            {
                ArrayPool<char>.Shared.Return(charBuffer);
                ArrayPool<char>.Shared.Return(tempBuffer);
            }
        }

        /// <summary>
        /// Reads all the valid Brainf*ck/PBrain operators from a source <see cref="ReadOnlySpan{T}"/> and writes them into a target <see cref="Span{T}"/>
        /// </summary>
        /// <param name="source">The source <see cref="ReadOnlySpan{T}"/> with the characters to read</param>
        /// <param name="destination">The target <see cref="Span{T}"/> to write the parsed operators to</param>
        /// <returns>The number of Brainf*ck/PBrain operators that have been parsed and written to <paramref name="destination"/></returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ExtractOperators(ReadOnlySpan<char> source, Span<char> destination)
        {
            DebugGuard.MustBeGreaterThan(source.Length, 0, nameof(source));
            DebugGuard.MustBeGreaterThan(destination.Length, 0, nameof(destination));

            ref char sourceRef = ref MemoryMarshal.GetReference(source);
            ref char destinationRef = ref MemoryMarshal.GetReference(destination);
            int sourceLength = source.Length;
            int destinationLength = destination.Length;
            int j = 0;

            for (int i = 0; i < sourceLength; i++)
            {
                char c = Unsafe.Add(ref sourceRef, i);

                // Ignore characters that are not valid operators (comments)
                if (!Brainf_ckParser.IsOperator(c)) continue;

                Unsafe.Add(ref destinationRef, j++) = c;
                
                // Stop if the destination span is full
                if (j == destinationLength) return destinationLength;
            }

            return j;
        }
    }
}
