using System;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Shared.Models.Ide;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Brainf_ckSharp.Shared.ViewModels.Views.Abstract
{
    /// <summary>
    /// A <see cref="ViewModelBase"/> implementation for workspaces
    /// </summary>
    public abstract class WorkspaceViewModelBase : ViewModelBase
    {
        private SourceCode _Code = SourceCode.CreateEmpty();

        /// <summary>
        /// Gets or sets the loaded source code
        /// </summary>
        public SourceCode Code
        {
            get => _Code;
            protected set => Set(ref _Code, value);
        }

        private ReadOnlyMemory<char> _Text = SourceCode.EmptyContent.AsMemory();

        /// <summary>
        /// Gets or sets the currently displayed text
        /// </summary>
        public ReadOnlyMemory<char> Text
        {
            get => _Text;
            set
            {
                if (_Text.Span.SequenceEqual(value.Span)) return;

                _Text = value;

                OnPropertyChanged();

                IsUnsavedEditPending = !value.Span.SequenceEqual(Code.Content.AsSpan());

                OnTextChanged();
            }
        }

        private bool _IsUnsavedEditPending;

        /// <summary>
        /// Gets whether or not there are pending unsaved changes to the current file
        /// </summary>
        public bool IsUnsavedEditPending
        {
            get => _IsUnsavedEditPending;
            private set => Set(ref _IsUnsavedEditPending, value);
        }

        private SyntaxValidationResult _ValidationResult;

        /// <summary>
        /// Gets the current <see cref="SyntaxValidationResult"/> value for <see cref="Text"/>
        /// </summary>
        public SyntaxValidationResult ValidationResult
        {
            get => _ValidationResult;
            set => Set(ref _ValidationResult, value);
        }

        private int _Row = 1;

        /// <summary>
        /// Gets the current row in the document in use
        /// </summary>
        public int Row
        {
            get => _Row;
            set
            {
                Guard.IsGreaterThan(value, 0, nameof(Row));

                Set(ref _Row, value);
            }
        }

        private int _Column = 1;

        /// <summary>
        /// Gets the current column in the document in use
        /// </summary>
        public int Column
        {
            get => _Column;
            set
            {
                Guard.IsGreaterThan(value, 0, nameof(Column));

                Set(ref _Column, value);
            }
        }

        /// <summary>
        /// Raised whenever <see cref="Text"/> changes
        /// </summary>
        protected virtual void OnTextChanged() { }
    }
}
