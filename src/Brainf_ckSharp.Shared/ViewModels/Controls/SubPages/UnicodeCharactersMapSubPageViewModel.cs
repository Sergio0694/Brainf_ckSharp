﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Brainf_ckSharp.Shared.Models;
using Microsoft.Toolkit.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nito.AsyncEx;

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages;

public sealed class UnicodeCharactersMapSubPageViewModel : ObservableObject
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
    /// Creates a new <see cref="UnicodeCharactersMapSubPageViewModel"/> instance
    /// </summary>
    public UnicodeCharactersMapSubPageViewModel()
    {
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
    }

    /// <summary>
    /// Gets the <see cref="ICommand"/> instance responsible for loading the available source codes
    /// </summary>
    public ICommand LoadDataCommand { get; }

    /// <summary>
    /// Gets the current collection of intervals to display
    /// </summary>
    public ObservableGroupedCollection<UnicodeInterval, UnicodeCharacter> Source { get; } = new();

    /// <summary>
    /// Loads the grouped characters to display
    /// </summary>
    public async Task LoadDataAsync()
    {
        using (await LoadingMutex.LockAsync())
        {
            // Load the first group if needed
            var first = _32To127 ??= await Task.Run(() => (
                from i in Enumerable.Range(0, 128 - 32)
                let c = (char)(i + 32)
                select new UnicodeCharacter(c)).ToArray());

            Source.Add(new ObservableGroup<UnicodeInterval, UnicodeCharacter>(
                new UnicodeInterval(0, 31),
                first));

            // Load the second group if needed
            var second = _160To255 ??= await Task.Run(() => (
                from i in Enumerable.Range(0, 256 - 160)
                let c = (char)(i + 160)
                select new UnicodeCharacter(c)).ToArray());

            Source.Add(new ObservableGroup<UnicodeInterval, UnicodeCharacter>(
                new UnicodeInterval(128, 159),
                second));
        }
    }
}
