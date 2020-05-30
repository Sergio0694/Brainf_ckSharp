using System;
using Brainf_ckSharp.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Brainf_ckSharp.Shared.ViewModels.Views.Abstract
{
    /// <summary>
    /// A <see cref="ViewModelBase"/> implementation for workspaces
    /// </summary>
    public abstract class WorkspaceViewModelBase : ViewModelBase
    {
        /// <summary>
        /// Gets or sets the currently displayed text
        /// </summary>
        public abstract ReadOnlyMemory<char> Text { get; set; }

        private SyntaxValidationResult _ValidationResult = Brainf_ckParser.ValidateSyntax(string.Empty);

        /// <summary>
        /// Gets the current <see cref="SyntaxValidationResult"/> value for <see cref="Text"/>
        /// </summary>
        public SyntaxValidationResult ValidationResult
        {
            get => _ValidationResult;
            set => Set(ref _ValidationResult, value);
        }

        private bool _IsUnsavedEditPending;

        /// <summary>
        /// Gets whether or not there are pending unsaved changes to the current file
        /// </summary>
        public bool IsUnsavedEditPending
        {
            get => _IsUnsavedEditPending;
            protected set => Set(ref _IsUnsavedEditPending, value);
        }

        private int _Row;

        /// <summary>
        /// Gets the current row in the document in use
        /// </summary>
        public int Row
        {
            get => _Row;
            set => Set(ref _Row, value);
        }

        private int _Column;

        /// <summary>
        /// Gets the current column in the document in use
        /// </summary>
        public int Column
        {
            get => _Column;
            set => Set(ref _Column, value);
        }
    }
}
