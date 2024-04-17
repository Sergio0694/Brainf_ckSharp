using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Enums;
using Brainf_ckSharp.Shared.Messages.Ide;
using Brainf_ckSharp.Shared.Models;
using Brainf_ckSharp.Shared.Models.Ide;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages;

/// <summary>
/// A viewmodel for the code library page.
/// </summary>
public sealed partial class CodeLibrarySubPageViewModel : ObservableRecipient
{
    /// <summary>
    /// The relative path of folder that contains the sample files
    /// </summary>
    private const string SampleFilesRelativePath = @"Assets\Samples\";

    /// <summary>
    /// The <see cref="IComparer{T}"/> instance to compare <see cref="Source"/> items.
    /// </summary>
    private static readonly IComparer<object?> SourceItemEditTimeComparer = Comparer<object?>.Create(CompareSourceItemByEditTime);

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
    /// The cached collection of sample codes, if available.
    /// </summary>
    private static IReadOnlyList<CodeLibraryEntry>? sampleCodes;

    /// <summary>
    /// The <see cref="IAnalyticsService"/> instance currently in use
    /// </summary>
    private readonly IAnalyticsService analyticsService;

    /// <summary>
    /// The <see cref="IFilesService"/> instance currently in use
    /// </summary>
    private readonly IFilesService filesService;

    /// <summary>
    /// The <see cref="IFilesHistoryService"/> instance currently in use
    /// </summary>
    private readonly IFilesHistoryService filesHistoryService;

    /// <summary>
    /// The <see cref="IClipboardService"/> instance currently in use
    /// </summary>
    private readonly IClipboardService clipboardService;

    /// <summary>
    /// The <see cref="IShareService"/> instance currently in use
    /// </summary>
    private readonly IShareService shareService;

    /// <summary>
    /// Loads the available code samples
    /// </summary>
    /// <returns>A <see cref="IReadOnlyList{T}"/> instance with the loaded code samples</returns>
    private async ValueTask<IReadOnlyList<CodeLibraryEntry>> GetSampleCodesAsync()
    {
        return sampleCodes ??= await Task.WhenAll(SampleFilesMapping.Select(async item =>
        {
            string installationPath = this.filesService.InstallationPath;
            string path = Path.Combine(installationPath, SampleFilesRelativePath, $"{item.Filename}.txt");
            IFile file = await this.filesService.GetFileFromPathAsync(path);

            CodeLibraryEntry? entry = await CodeLibraryEntry.TryLoadFromFileAsync(file, item.Title);

            if (entry is null) ThrowHelper.ThrowInvalidOperationException("Failed to load the requested sample");

            return entry;
        }));
    }

    /// <summary>
    /// Creates a new <see cref="CodeLibrarySubPageViewModel"/> instance
    /// </summary>
    /// <param name="messenger">The <see cref="IMessenger"/> instance to use</param>
    /// <param name="analyticsService">The <see cref="IAnalyticsService"/> instance to use</param>
    /// <param name="filesService">The <see cref="IFilesService"/> instance to use</param>
    /// <param name="filesHistoryService">The <see cref="IFilesHistoryService"/> instance to use</param>
    /// <param name="clipboardService">The <see cref="IClipboardService"/> instance to use</param>
    /// <param name="shareService">The <see cref="IShareService"/> instance to use</param>
    public CodeLibrarySubPageViewModel(
        IMessenger messenger,
        IAnalyticsService analyticsService,
        IFilesService filesService,
        IFilesHistoryService filesHistoryService,
        IClipboardService clipboardService,
        IShareService shareService)
        : base(messenger)
    {
        this.analyticsService = analyticsService;
        this.filesService = filesService;
        this.filesHistoryService = filesHistoryService;
        this.clipboardService = clipboardService;
        this.shareService = shareService;
    }

    /// <summary>
    /// Gets the current collection of sections to display
    /// </summary>
    public ObservableGroupedCollection<CodeLibrarySection, object> Source { get; } = new();

    /// <summary>
    /// Loads the currently available code samples and recently used files
    /// </summary>
    [RelayCommand]
    private async Task LoadDataAsync()
    {
        List<CodeLibraryEntry> recent = new();

        // Load the recent files
        await foreach ((IFile file, string data) in this.filesService.GetFutureAccessFilesAsync())
        {
            // Deserialize the metadata and prepare the model
            CodeMetadata? metadata = string.IsNullOrEmpty(data) ? new() : JsonSerializer.Deserialize(data, Brainf_ckSharpJsonSerializerContext.Default.CodeMetadata);

            if (metadata is null) ThrowHelper.ThrowInvalidOperationException("Failed to load the source code metadata");

            CodeLibraryEntry? entry = await CodeLibraryEntry.TryLoadFromFileAsync(file, metadata);

            if (entry is null) ThrowHelper.ThrowInvalidOperationException("Failed to load the source code file");

            recent.Add(entry);
        }

        // Sort chronologically
        IReadOnlyList<CodeLibraryEntry> sorted = recent.OrderByDescending(entry => entry.EditTime).ToArray();

        // Load the code samples
        IReadOnlyList<CodeLibraryEntry> samples = await GetSampleCodesAsync();

        // Add the favorites, if any
        IEnumerable<CodeLibraryEntry> favorited = sorted.Where(entry => entry.Metadata.IsFavorited)!;
        _ = Source.AddGroup(CodeLibrarySection.Favorites, favorited.Append<object>(CodeLibrarySection.Favorites));

        // Add the recent and sample items
        IEnumerable<CodeLibraryEntry> unfavorited = sorted.Where(entry => !entry.Metadata.IsFavorited)!;
        _ = Source.AddGroup(CodeLibrarySection.Recent, unfavorited.Append<object>(CodeLibrarySection.Recent));
        _ = Source.AddGroup(CodeLibrarySection.Samples, samples);
    }

    /// <summary>
    /// Processes a given item
    /// </summary>
    /// <param name="item">The target item to process</param>
    [RelayCommand]
    private void ProcessItem(object? item)
    {
        if (item is CodeLibraryEntry entry) _ = OpenFileAsync(entry);
        else if (item is CodeLibrarySection c) RequestOpenFile(c == CodeLibrarySection.Favorites);
        else ThrowHelper.ThrowArgumentException("The input item is not valid");
    }

    /// <summary>
    /// Requests to pick and open a source code file
    /// </summary>
    private void RequestOpenFile(bool favorite)
    {
        _ = Messenger.Send(new PickOpenFileRequestMessage(favorite));
    }

    /// <summary>
    /// Sends a request to load a specified code entry
    /// </summary>
    /// <param name="entry">The selected <see cref="CodeLibraryEntry"/> model</param>
    private async Task OpenFileAsync(CodeLibraryEntry entry)
    {
        if (entry.File.IsReadOnly)
        {
            this.analyticsService.Log(EventNames.SampleCodeSelected, (nameof(SourceCode.File), entry.File.DisplayName));

            SourceCode code = await SourceCode.LoadFromReferenceFileAsync(entry.File);

            _ = Messenger.Send(new LoadSourceCodeRequestMessage(code));
        }
        else
        {
            if (await SourceCode.TryLoadFromEditableFileAsync(entry.File) is not SourceCode sourceCode) return;

            _ = Messenger.Send(new LoadSourceCodeRequestMessage(sourceCode));
        }
    }

    /// <summary>
    /// Toggles the favorite state of a given <see cref="CodeLibraryEntry"/> instance
    /// </summary>
    /// <param name="entry">The <see cref="CodeLibraryEntry"/> instance to toggle</param>
    [RelayCommand]
    private void ToggleFavorite(CodeLibraryEntry? entry)
    {
        Guard.IsNotNull(entry);

        this.analyticsService.Log(EventNames.ToggleFavoriteSourceCode, (nameof(CodeMetadata.IsFavorited), entry.Metadata.IsFavorited.ToString()));

        // If the current item is favorited, set is as not favorited
        // and move it back into the recent files section.
        // If the favorites section becomes empty, remove it entirely.
        if (entry.Metadata.IsFavorited)
        {
            entry.Metadata.IsFavorited = false;

            Source.RemoveItem(CodeLibrarySection.Favorites, entry);
            _ = Source.InsertItem(CodeLibrarySection.Recent, Comparer<CodeLibrarySection>.Default, entry, SourceItemEditTimeComparer);
        }
        else
        {
            entry.Metadata.IsFavorited = true;

            Source.RemoveItem(CodeLibrarySection.Recent, entry);
            _ = Source.InsertItem(CodeLibrarySection.Favorites, Comparer<CodeLibrarySection>.Default, entry, SourceItemEditTimeComparer);
        }

        entry.File.RequestFutureAccessPermission(JsonSerializer.Serialize(entry.Metadata, Brainf_ckSharpJsonSerializerContext.Default.CodeMetadata));
    }

    /// <summary>
    /// Copies the content of a specified entry to the clipboard
    /// </summary>
    /// <param name="entry">The <see cref="CodeLibraryEntry"/> instance to copy to the clipboard</param>
    [RelayCommand]
    private async Task CopyToClipboardAsync(CodeLibraryEntry? entry)
    {
        Guard.IsNotNull(entry);

        this.analyticsService.Log(EventNames.CopySourceCode);

        string text = await entry.File.ReadAllTextAsync();

        _ = this.clipboardService.TryCopy(text);
    }

    /// <summary>
    /// Shares a specified entry
    /// </summary>
    /// <param name="entry">The <see cref="CodeLibraryEntry"/> instance to share</param>
    [RelayCommand]
    private void Share(CodeLibraryEntry? entry)
    {
        Guard.IsNotNull(entry);

        this.analyticsService.Log(EventNames.ShareSourceCode);

        this.shareService.Share(entry.Title, entry.File);
    }

    /// <summary>
    /// Removes a source code from the library and removes the relative permissions
    /// </summary>
    /// <param name="entry">The <see cref="CodeLibraryEntry"/> instance to remove</param>
    private void RemoveTrackedSourceCode(CodeLibraryEntry entry)
    {
        ObservableGroup<CodeLibrarySection, object> group = Source.First<ObservableGroup<CodeLibrarySection, object>>(g => g.Contains(entry));

        if (group.Count == 1) _ = Source.Remove(group);
        else _ = group.Remove(entry);

        entry.File.RemoveFutureAccessPermission();
    }

    /// <summary>
    /// Removes a specific <see cref="CodeLibraryEntry"/> instance from the code library
    /// </summary>
    /// <param name="entry">The <see cref="CodeLibraryEntry"/> instance to remove</param>
    [RelayCommand]
    private Task RemoveFromLibraryAsync(CodeLibraryEntry? entry)
    {
        Guard.IsNotNull(entry);

        this.analyticsService.Log(EventNames.RemoveFromLibrary, (nameof(CodeMetadata.IsFavorited), entry.Metadata.IsFavorited.ToString()));

        RemoveTrackedSourceCode(entry);

        return this.filesHistoryService.RemoveActivityAsync(entry.File);
    }

    /// <summary>
    /// Deletes a specific <see cref="CodeLibraryEntry"/> instance in the code library
    /// </summary>
    /// <param name="entry">The <see cref="CodeLibraryEntry"/> instance to delete</param>
    [RelayCommand]
    private Task DeleteAsync(CodeLibraryEntry? entry)
    {
        Guard.IsNotNull(entry);

        this.analyticsService.Log(EventNames.DeleteSourceCode, (nameof(CodeMetadata.IsFavorited), entry.Metadata.IsFavorited.ToString()));

        RemoveTrackedSourceCode(entry);

        // We can remove the item from history and delete the file in parallel,
        // since removing a tracked item from history requires no file access.
        return Task.WhenAll(
            this.filesHistoryService.RemoveActivityAsync(entry.File),
            entry.File.DeleteAsync());
    }

    /// <summary>
    /// Compares two <see cref="Source"/> items by their edit time, assuming they are <see cref="CodeLibraryEntry"/> instances.
    /// </summary>
    /// <param name="a">The first item to compare.</param>
    /// <param name="b">The second iteme to compare.</param>
    /// <returns>The comparison result.</returns>
    private static int CompareSourceItemByEditTime(object? a, object? b)
    {
        if (a is CodeLibraryEntry left && b is CodeLibraryEntry right)
        {
            return left.EditTime.CompareTo(right.EditTime);
        }

        return -1;
    }
}
