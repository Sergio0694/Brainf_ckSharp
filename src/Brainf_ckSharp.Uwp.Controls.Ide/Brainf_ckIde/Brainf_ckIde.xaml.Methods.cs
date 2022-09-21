using System;
using System.Buffers;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.System;
using Windows.UI.Text;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Uwp.Controls.Ide.Extensions.System;
using Brainf_ckSharp.Uwp.Controls.Ide.Extensions.Windows.UI.Text;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.HighPerformance;
using Microsoft.Toolkit.HighPerformance.Buffers;

namespace Brainf_ckSharp.Uwp.Controls.Ide;

public sealed partial class Brainf_ckIde
{
    /// <summary>
    /// The loaded text currently in use (used as a reference for changes)
    /// </summary>
    private string _LoadedText = "\r";

    /// <summary>
    /// Loads a given text file and starts using it as reference for the git diff indicators
    /// </summary>
    /// <param name="text"></param>
    public void LoadText(string text)
    {
        _LoadedText = text.WithCarriageReturnLineEndings();

        _DiffIndicators.Span.Clear();

        BreakpointIndicators.Clear();

        CodeEditBox.Document.LoadFromString(text);
    }

    /// <summary>
    /// Marks the currently loaded text file as being saved and updates the git diff indicators
    /// </summary>
    public void MarkTextAsSaved()
    {
        _LoadedText = CodeEditBox.Text;

        UpdateDiffInfo();

        IdeOverlaysCanvas.Invalidate();
    }

    /// <summary>
    /// Types a new character into the current document
    /// </summary>
    /// <param name="character">The character to type</param>
    public void TypeCharacter(char character)
    {
        CodeEditBox.Document.Selection.TypeText(character.ToString());
    }

    /// <summary>
    /// Inserts a given source <see cref="string"/> into the current selection
    /// </summary>
    /// <param name="source">The source text to insert</param>
    public void InsertText(string source)
    {
        CodeEditBox.InsertText(source);
    }

    /// <summary>
    /// Deletes the last character as if the delete key had been pressed
    /// </summary>
    public void DeleteCharacter()
    {
        CodeEditBox.DeleteSelectionOrCharacter();
    }

    /// <summary>
    /// Moves the current cursor
    /// </summary>
    /// <param name="key">The <see cref="VirtualKey"/> value indicating the direction to move to</param>
    public void Move(VirtualKey key)
    {
        switch (key)
        {
            case VirtualKey.Up:
                CodeEditBox.Document.Selection.MoveUp(TextRangeUnit.Line, 1, false);
                break;
            case VirtualKey.Down:
                CodeEditBox.Document.Selection.MoveDown(TextRangeUnit.Line, 1, false);
                break;
            case VirtualKey.Left:
                CodeEditBox.Document.Selection.MoveLeft(TextRangeUnit.Character, 1, false);
                break;
            case VirtualKey.Right:
                CodeEditBox.Document.Selection.MoveRight(TextRangeUnit.Character, 1, false);
                break;
            default:
                ThrowHelper.ThrowArgumentException(nameof(key), "Invalid virtual key");
                break;
        }
    }

    /// <summary>
    /// Moves the current cursor to a specified position
    /// </summary>
    /// <param name="row">The target row</param>
    /// <param name="column">The target column</param>
    public void Move(int row, int column)
    {
        int index = Text.CalculateIndex(row, column);

        CodeEditBox.Document.Selection.SetRange(index, index);
    }

    /// <summary>
    /// Gets a buffer with the existing breakpoints (their line numbers)
    /// </summary>
    /// <returns>A <see cref="MemoryOwner{T}"/> instance with the line numbers with a breakpoint.</returns>
    [Pure]
    public IMemoryOwner<int> GetBreakpoints()
    {
        int count = BreakpointIndicators.Count;

        if (count == 0) return MemoryOwner<int>.Empty;

        // Rent a buffer to copy the line numbers with a breakpoint
        MemoryOwner<int> buffer = MemoryOwner<int>.Allocate(count);

        ref int bufferRef = ref buffer.DangerousGetReference();
        int i = 0;

        // Copy the existing breakpoints
        foreach (var pair in BreakpointIndicators)
        {
            Unsafe.Add(ref bufferRef, i++) = pair.Key - 1;
        }

        // Get the underlying array to sort in-place.
        // This will no longer be needed on .NET 5, as there are APIs
        // to sort items within a Span<T> directly, not yet on UWP though.
        _ = MemoryMarshal.TryGetArray<int>(buffer.Memory, out var segment);

        Array.Sort(segment.Array!, segment.Offset, segment.Count);

        // We're tracking the current position within the breakpoints buffer,
        // the current line number and the absolute offset within the text.
        i = 0;
        int j = 0, k = 0;

        foreach (var line in CodeEditBox.Text.Tokenize(Characters.CarriageReturn))
        {
            // If the current line is marked, do a linear search to find the first operator
            if (Unsafe.Add(ref bufferRef, i) == j)
            {
                foreach (var item in line.Enumerate())
                {
                    if (Brainf_ckParser.IsOperator(item.Value))
                    {
                        Unsafe.Add(ref bufferRef, i++) = k + item.Index;

                        break;
                    }
                }
            }

            // Increment the line number and the absolute offset (line length and \r character)
            j++;
            k += line.Length + 1;
        }

        return buffer;
    }

    /// <summary>
    /// Shows the syntax error tooltip, if an error is present
    /// </summary>
    public void TryShowSyntaxErrorToolTip()
    {
        CodeEditBox.TryShowSyntaxErrorToolTip();
    }
}
