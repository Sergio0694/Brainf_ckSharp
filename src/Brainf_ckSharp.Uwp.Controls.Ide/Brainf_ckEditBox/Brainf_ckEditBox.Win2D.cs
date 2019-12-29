using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Brainf_ckSharp.Buffers;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Helpers;
using Brainf_ckSharp.Uwp.Controls.Ide.Models;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    public sealed partial class Brainf_ckEditBox
    {
        /// <summary>
        /// The current sequence of bracket pairs being displayed in the text
        /// </summary>
        private MemoryOwner<(int Start, int End)> _BracketPairs = MemoryOwner<(int, int)>.Allocate(0);

        /// <summary>
        /// The current sequence of column guide coordinates to render
        /// </summary>
        private MemoryOwner<ColumnGuideInfo> _ColumnGuides = MemoryOwner<ColumnGuideInfo>.Allocate(0);

        /// <summary>
        /// The current sequence of indices for the space characters to render
        /// </summary>
        private MemoryOwner<int> _SpaceIndices = MemoryOwner<int>.Allocate(0);

        /// <summary>
        /// The current sequence of areas for the space characters to render
        /// </summary>
        private MemoryOwner<Rect> _SpaceAreas = MemoryOwner<Rect>.Allocate(0);

        /// <summary>
        /// The current sequence of indices for the tab characters to render
        /// </summary>
        private MemoryOwner<int> _TabIndices = MemoryOwner<int>.Allocate(0);

        /// <summary>
        /// The current sequence of areas for the tab characters to render
        /// </summary>
        private MemoryOwner<Rect> _TabAreas = MemoryOwner<Rect>.Allocate(0);

        /// <summary>
        /// Draws the text overlays when an update is requested
        /// </summary>
        /// <param name="sender">The sender <see cref="CanvasControl"/> instance</param>
        /// <param name="args">The <see cref="CanvasDrawEventArgs"/> for the current instance</param>
        private void TextOverlaysCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            CanvasStrokeStyle style = new CanvasStrokeStyle { CustomDashStyle = new float[] { 2, 4 } };

            // Spaces
            foreach (Rect spaceArea in _SpaceAreas.Span)
            {
                Rect dot = new Rect
                {
                    Height = 2,
                    Width = 2,
                    X = spaceArea.Left + (spaceArea.Right - spaceArea.Left) / 2 + 3,
                    Y = spaceArea.Top + (spaceArea.Bottom - spaceArea.Top) / 2 - 1
                };
                args.DrawingSession.FillRectangle(dot, Colors.DimGray);
            }

            // Tabs
            foreach (Rect tabArea in _TabAreas.Span)
            {
                double width = tabArea.Right - tabArea.Left;
                if (width < 12)
                {
                    // Small dot at the center
                    Rect dot = new Rect
                    {
                        Height = 2,
                        Width = 2,
                        X = tabArea.Left + width / 2 + 5,
                        Y = tabArea.Top + (tabArea.Bottom - tabArea.Top) / 2 - 1
                    };
                    args.DrawingSession.FillRectangle(dot, Colors.DimGray);
                }
                else
                {
                    // Arrow indicator
                    float
                        x = (float)(tabArea.Left + width / 2),
                        y = (float)(tabArea.Top + (tabArea.Bottom - tabArea.Top) / 2 - 2);
                    int length = width < 28 ? 8 : 12;
                    args.DrawingSession.DrawLine(x, y + 2, x + length, y + 2, Colors.DimGray);
                    args.DrawingSession.DrawLine(x + length - 2, y, x + length, y + 2, Colors.DimGray);
                    args.DrawingSession.DrawLine(x + length - 2, y + 4, x + length, y + 2, Colors.DimGray);
                }
            }

            // Column guides
            foreach (ColumnGuideInfo guideInfo in _ColumnGuides.Span)
            {
                args.DrawingSession.DrawLine(
                    guideInfo.X + 0.5f,
                    guideInfo.Y - 0.5f,
                    guideInfo.X + 0.5f,
                    guideInfo.Y + guideInfo.Height + 0.5f,
                    Colors.LightGray,
                    1,
                    style);
            }
        }

        /// <summary>
        /// Tries to update the current sequences of spaces and tabs in the text
        /// </summary>
        /// <returns><see langword="true"/> if the sequences were updated, <see langword="false"/> otherwise</returns>
        [Pure]
        private bool TryUpdateWhitespaceCharactersList()
        {
            // Prepare the current text
            ReadOnlySpan<char> text = _Text.AsSpan();
            ref char r0 = ref MemoryMarshal.GetReference(text);
            int length = text.Length;

            // Target buffers
            MemoryOwner<int>
                spaces = MemoryOwner<int>.Allocate(length),
                tabs = MemoryOwner<int>.Allocate(length);
            ref int spacesRef = ref spaces.GetReference();
            ref int tabsRef = ref tabs.GetReference();
            int
                spacesCount = 0,
                tabsCount = 0;

            // Go through the executable
            for (int i = 0; i < length; i++)
            {
                switch (Unsafe.Add(ref r0, i))
                {
                    case ' ':
                        Unsafe.Add(ref spacesRef, spacesCount++) = i;
                        break;
                    case '\t':
                        Unsafe.Add(ref tabsRef, tabsCount++) = i;
                        break;
                }
            }

            // Update the current data
            _SpaceIndices.Dispose();
            _SpaceIndices = spaces.Slice(spacesCount);
            _TabIndices.Dispose();
            _TabIndices = tabs.Slice(tabsCount);

            return true;
        }

        /// <summary>
        /// Processes the current whitespace info and updates <see cref="_SpaceAreas"/> and <see cref="_TabAreas"/>
        /// </summary>
        private void ProcessWhitespaceData()
        {
            _SpaceAreas.Dispose();

            // Spaces first
            MemoryOwner<int> spaceIndices = _SpaceIndices;
            MemoryOwner<Rect> spaceAreas = MemoryOwner<Rect>.Allocate(spaceIndices.Size);

            _SpaceAreas = spaceAreas;

            // Skip if there are no spaces
            if (spaceAreas.Size > 0)
            {
                ref int spaceIndicesRef = ref spaceIndices.GetReference();
                ref Rect spaceAreasRef = ref spaceAreas.GetReference();

                for (int i = 0; i < spaceIndices.Size; i++)
                {
                    int index = Unsafe.Add(ref spaceIndicesRef, i);
                    ITextRange range = Document.GetRange(index, index + 1);
                    range.GetRect(PointOptions.Transform, out Unsafe.Add(ref spaceAreasRef, i), out _);
                }
            }

            _TabAreas.Dispose();

            // Then tabs
            MemoryOwner<int> tabIndices = _TabIndices;
            MemoryOwner<Rect> tabAreas = MemoryOwner<Rect>.Allocate(tabIndices.Size);

            _TabAreas = tabAreas;

            // Skip if there are no tabs
            if (tabAreas.Size > 0)
            {
                ref int tabIndicesRef = ref tabIndices.GetReference();
                ref Rect tabAreasRef = ref tabAreas.GetReference();

                for (int i = 0; i < tabIndices.Size; i++)
                {
                    int index = Unsafe.Add(ref tabIndicesRef, i);
                    ITextRange range = Document.GetRange(index, index + 1);
                    range.GetRect(PointOptions.Transform, out Unsafe.Add(ref tabAreasRef, i), out _);
                }
            }
        }

        /// <summary>
        /// Tries to update the current sequence of brackets displayed in the text
        /// </summary>
        /// <returns><see langword="true"/> if the sequence was updated, <see langword="false"/> otherwise</returns>
        [Pure]
        private bool TryUpdateBracketsList()
        {
            DebugGuard.MustBeTrue(_IsSyntaxValid, nameof(_IsSyntaxValid));

            // Prepare the current text
            ReadOnlySpan<char> text = _Text.AsSpan();
            ref char r0 = ref MemoryMarshal.GetReference(text);
            int length = text.Length;

            // Temporary buffers, just in the original method in the core library
            int tempBuffersLength = length / 2 + 1;
            using UnsafeSpan<(int, int)> rootTempIndices = UnsafeSpan<(int, int)>.Allocate(tempBuffersLength);
            using UnsafeSpan<(int, int)> functionTempIndices = UnsafeSpan<(int, int)>.Allocate(tempBuffersLength);
            ref (int Index, int Y) rootTempIndicesRef = ref rootTempIndices.GetReference();
            ref (int Index, int Y) functionTempIndicesRef = ref functionTempIndices.GetReference();
            int
                jumps = 0,
                y = 0;

            // Target buffer
            MemoryOwner<(int, int)> jumpTable = MemoryOwner<(int, int)>.Allocate(length);
            ref (int, int) jumpTableRef = ref jumpTable.GetReference();

            // Go through the executable to build the jump table for each open parenthesis or square bracket
            for (int r = 0, f = -1, i = 0; i < length; i++)
            {
                switch (Unsafe.Add(ref r0, i))
                {
                    // Track each loop start
                    case Characters.LoopStart:
                        if (f == -1) Unsafe.Add(ref rootTempIndicesRef, r++) = (i, y);
                        else Unsafe.Add(ref functionTempIndicesRef, f++) = (i, y);
                        break;

                    // Track loop ends if there is at least one line of difference
                    case Characters.LoopEnd:
                        var start = f == -1
                            ? Unsafe.Add(ref rootTempIndicesRef, --r)
                            : Unsafe.Add(ref functionTempIndicesRef, --f);
                        if (start.Y < y)
                        {
                            Unsafe.Add(ref jumpTableRef, jumps++) = (start.Index, i);
                        }
                        break;

                    // Track functions, starting at 1 so that index 0 stores the function brackets
                    case Characters.FunctionStart:
                        f = 1;
                        functionTempIndicesRef = (i, y);
                        break;
                    case Characters.FunctionEnd:
                        f = -1;
                        if (functionTempIndicesRef.Y < y)
                        {
                            Unsafe.Add(ref jumpTableRef, jumps++) = (functionTempIndicesRef.Index, i);
                        }
                        break;

                    // Track each new line
                    case '\r':
                        y++;
                        break;
                }
            }

            // Skip the update if both sequences match
            if (_BracketPairs.AsBytes().SequenceEqual(jumpTable.AsBytes()))
            {
                jumpTable.Dispose();
                return false;
            }

            // Update the current brackets sequence
            _BracketPairs.Dispose();
            _BracketPairs = jumpTable.Slice(jumps);

            return true;
        }

        /// <summary>
        /// Processes the current column guides info and updates <see cref="_ColumnGuides"/>
        /// </summary>
        private void ProcessColumnGuides()
        {
            _ColumnGuides.Dispose();

            // Update the target buffer
            MemoryOwner<(int, int)> bracketPairs = _BracketPairs;
            MemoryOwner<ColumnGuideInfo> columnGuides = MemoryOwner<ColumnGuideInfo>.Allocate(bracketPairs.Size);

            _ColumnGuides = columnGuides;

            // Skip if there are no brackets to render
            if (columnGuides.Size == 0) return;

            float offset = (float)Padding.Top + 22;
            ref (int Start, int End) bracketPairsRef = ref bracketPairs.GetReference();
            ref ColumnGuideInfo columnGuidesRef = ref columnGuides.GetReference();

            for (int i = 0; i < bracketPairs.Size; i++)
            {
                // Get the initial and ending range
                var bounds = Unsafe.Add(ref bracketPairsRef, i);
                ITextRange range = Document.GetRangeAt(bounds.Start);
                range.GetRect(PointOptions.Transform, out Rect open, out _);
                range = Document.GetRangeAt(bounds.End);
                range.GetRect(PointOptions.Transform, out Rect close, out _);

                // Render the new line guide
                ColumnGuideInfo guideInfo = new ColumnGuideInfo(
                    MathF.Min((float)open.X, (float)close.X) + 10,
                    (float)open.Top + offset,
                    (float)(close.Top - open.Bottom));
                Unsafe.Add(ref columnGuidesRef, i) = guideInfo;
            }
        }
    }
}
