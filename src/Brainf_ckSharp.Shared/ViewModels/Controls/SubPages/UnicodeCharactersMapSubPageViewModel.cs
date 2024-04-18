using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Brainf_ckSharp.Shared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nito.AsyncEx;
using CommunityToolkit.Mvvm.Collections;

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages;

/// <summary>
/// A viewmodel for the characters control.
/// </summary>
public sealed partial class UnicodeCharactersMapSubPageViewModel : ObservableObject
{
    /// <summary>
    /// A mutex to avoid race conditions when loading <see cref="_32To127"/> and <see cref="_160To255"/>
    /// </summary>
    private static readonly AsyncLock LoadingMutex = new();

    /// <summary>
    /// The collection of characters in the [32, 127] range
    /// </summary>
    private static IReadOnlyList<UnicodeCharacter>? _32To127;

    /// <summary>
    /// The collection of characters in the [160, 255] range
    /// </summary>
    private static IReadOnlyList<UnicodeCharacter>? _160To255;

    /// <summary>
    /// Gets the current collection of intervals to display
    /// </summary>
    public ObservableGroupedCollection<UnicodeInterval, UnicodeCharacter> Source { get; } = [];

    /// <summary>
    /// Loads the grouped characters to display
    /// </summary>
    [RelayCommand]
    public async Task LoadDataAsync()
    {
        using (await LoadingMutex.LockAsync())
        {
            // Load the first group if needed
            IReadOnlyList<UnicodeCharacter> first = _32To127 ??= await Task.Run(() => (
                from i in Enumerable.Range(0, 128 - 32)
                let c = (char)(i + 32)
                select new UnicodeCharacter { Value = c }).ToArray());

            Source.Add(new ObservableGroup<UnicodeInterval, UnicodeCharacter>(
                new UnicodeInterval { Start = 0, End = 31 },
                first));

            // Load the second group if needed
            IReadOnlyList<UnicodeCharacter> second = _160To255 ??= await Task.Run(() => (
                from i in Enumerable.Range(0, 256 - 160)
                let c = (char)(i + 160)
                select new UnicodeCharacter { Value = c }).ToArray());

            Source.Add(new ObservableGroup<UnicodeInterval, UnicodeCharacter>(
                new UnicodeInterval { Start = 128, End = 159 },
                second));
        }
    }
}
