using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Text;
using Brainf_ck_sharp;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using Brainf_ck_sharp_UWP.DataModels.Misc.IDEIndentationGuides;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Messages;
using Brainf_ck_sharp_UWP.Messages.Actions;
using Brainf_ck_sharp_UWP.Messages.IDEStatus;
using Brainf_ck_sharp_UWP.ViewModels.Abstract;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.ViewModels
{
    public class IDEViewModel : ItemsCollectionViewModelBase<IDEIndentationLineInfo>
    {
        // The current document that's linked to the view
        private readonly ITextDocument Document;

        public IDEViewModel([NotNull] ITextDocument document) => Document = document;

        private bool _IsEnabled;

        /// <summary>
        /// Gets or sets whether or not the instance is enabled and it is processing incoming messages
        /// </summary>
        public bool IsEnabled
        {
            get => _IsEnabled;
            set
            {
                if (Set(ref _IsEnabled, value))
                {
                    if (value)
                    {
                        Messenger.Default.Register<OperatorAddedMessage>(this, op => InsertSingleCharacter(op.Operator));
                        Messenger.Default.Register<ClearScreenMessage>(this, m => TryClearScreen());
                        Messenger.Default.Register<PlayScriptMessage>(this, m => PlayRequested?.Invoke(this, m.StdinBuffer));
                        SendMessages();
                    }
                    else Messenger.Default.Unregister(this);
                }
            }
        }

        /// <summary>
        /// Raised whenever the user requests to play the current script
        /// </summary>
        public event EventHandler<String> PlayRequested;

        // Indicates whether or not the IDE contains at least a valid operator
        private bool _CanExecute;

        /// <summary>
        /// Sends the status info messages for the current state
        /// </summary>
        public void SendMessages([CanBeNull] String code = null)
        {
            if (code == null) Document.GetText(TextGetOptions.None, out code);
            Messenger.Default.Send(new ConsoleAvailableActionStatusChangedMessage(ConsoleAction.ClearScreen, code.Length > 1));
            (bool valid, int error) = Brainf_ckInterpreter.CheckSourceSyntax(code);
            (int row, int col) = code.FindCoordinates(Document.Selection.StartPosition);
            bool executable = Brainf_ckInterpreter.FindOperators(code);
            if (_CanExecute != executable)
            {
                Messenger.Default.Send(new IDEExecutableStatusChangedMessage(executable));
                _CanExecute = executable;
            }
            if (valid)
            {
                Messenger.Default.Send(new IDEStatusUpdateMessage(LocalizationManager.GetResource("Ready"), row, col, String.Empty));
            }
            else
            {
                (int y, int x) = code.FindCoordinates(error);
                Messenger.Default.Send(new IDEStatusUpdateMessage(LocalizationManager.GetResource("Warning"), row, col, y, x, String.Empty));
            }
        }

        /// <summary>
        /// Inserts a new character from the virtual keyboard and scrolls the current line into view, if needed
        /// </summary>
        /// <param name="c">The received character</param>
        private void InsertSingleCharacter(char c)
        {
            try
            {
                Document.Selection.SetText(TextSetOptions.None, c.ToString());
                Document.Selection.SetRange(Document.Selection.StartPosition + 1, Document.Selection.StartPosition + 1);
            }
            catch
            {
                //
            }
        }

        /// <summary>
        /// Clears the current content in the document
        /// </summary>
        private void TryClearScreen()
        {
            Document.SetText(TextSetOptions.None, String.Empty);
            SendMessages();
        }

        /// <summary>
        /// Updates the indentation info for a given state
        /// </summary>
        /// <param name="brackets">The collection of brackets and their position in the current text</param>
        public void UpdateIndentationInfo([CanBeNull] IReadOnlyList<(int, int, char)> brackets)
        {
            // // Check the info is available
            if (brackets == null || brackets.Count == 0)
            {
                Source.Clear();
                return;
            }
            int max = brackets.Max(entry => entry.Item1);

            // Updates the indentation info displayed on the IDE
            List<IDEIndentationLineInfo> source = new List<IDEIndentationLineInfo>();
            uint depth = 0;
            for (int i = 1; i <= max; i++)
            {
                IReadOnlyList<(int, int, char)> line = brackets.Where(info => info.Item1 == i).ToArray();
                if (line.Count == 0)
                {
                    source.Add(new IDEIndentationLineInfo(depth == 0 ? IDEIndentationInfoLineType.Empty : IDEIndentationInfoLineType.Straight));
                }
                else if (line[0].Item3 == '[')
                {
                    depth++;
                    source.Add(new IDEIndentationOpenBracketLineInfo(depth));
                }
                else if (line[0].Item3 == ']')
                {
                    depth--;
                    source.Add(new IDEIndentationLineInfo(IDEIndentationInfoLineType.ClosedBracket));
                }
            }
            
            // Update the source collection
            for (int i = 0; i < source.Count; i++)
            {
                // The source doesn't contain enough items
                if (Source.Count - 1 < i)
                {
                    Source.Add(source[i]);
                }

                // Replace the current item if needed
                IDEIndentationLineInfo
                    previous = Source[i],
                    next = source[i];

                if (previous is IDEIndentationOpenBracketLineInfo info &&
                    next is IDEIndentationOpenBracketLineInfo updated
                    ? info.Depth == updated.Depth
                    : previous.LineType == next.LineType)
                {
                    continue;
                }
                Source[i] = source[i];
            }

            // Remove the exceeding items
            int diff = Source.Count - source.Count;
            while (diff > 0)
            {
                Source.RemoveAt(Source.Count - 1);
                diff--;
            }
        }

        /// <summary>
        /// Gets the items collection for the current instance
        /// </summary>
        [NotNull]
        public ObservableCollection<GitDiffLineStatus> DiffStatusSource { get; } = new ObservableCollection<GitDiffLineStatus>();

        /// <summary>
        /// Updates the diff indicators for the current source code being edited
        /// </summary>
        /// <param name="previous">The previous code</param>
        /// <param name="current">The current code</param>
        public void UpdateGitDiffStatus([NotNull] String previous, [NotNull] String current)
        {
            // Clear the current indicators if the two strings are the same
            if (previous.Equals(current))
            {
                DiffStatusSource.Clear();
                return;
            }

            String[]
                currentLines = current.Split('\r'),
                previousLines = previous.Split('\r').Take(currentLines.Length).ToArray();
            List<GitDiffLineStatus> source = new List<GitDiffLineStatus>();
            for (int i = 0; i < currentLines.Length - 1; i++)
            {
                if (i > previousLines.Length - 1) source.Add(GitDiffLineStatus.Edited);
                else source.Add(currentLines[i].Equals(previousLines[i]) ? GitDiffLineStatus.Undefined : GitDiffLineStatus.Edited);
                // TODO: actually implement this
            }

            // Update the source collection
            for (int i = 0; i < source.Count; i++)
            {
                // The source doesn't contain enough items
                if (DiffStatusSource.Count - 1 < i)
                {
                    DiffStatusSource.Add(source[i]);
                }

                // Replace the current item if needed
                if (source[i] != DiffStatusSource[i])
                    DiffStatusSource[i] = source[i];
            }

            // Remove the exceeding items
            int diff = DiffStatusSource.Count - source.Count;
            while (diff > 0)
            {
                DiffStatusSource.RemoveAt(DiffStatusSource.Count - 1);
                diff--;
            }
        }
    }
}
