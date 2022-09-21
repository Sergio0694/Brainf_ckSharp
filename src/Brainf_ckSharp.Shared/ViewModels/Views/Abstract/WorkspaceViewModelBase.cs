using System;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Shared.Models.Ide;
using CommunityToolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace Brainf_ckSharp.Shared.ViewModels.Views.Abstract;

/// <summary>
/// An <see cref="ObservableRecipient"/> implementation for workspaces
/// </summary>
public abstract class WorkspaceViewModelBase : ObservableRecipient
{
    /// <summary>
    /// Creates a new <see cref="WorkspaceViewModelBase"/> instance
    /// </summary>
    /// <param name="messenger">The <see cref="IMessenger"/> instance to use</param>
    protected WorkspaceViewModelBase(IMessenger messenger) : base(messenger)
    {
    }

    private SourceCode _Code = SourceCode.CreateEmpty();

    /// <summary>
    /// Gets or sets the loaded source code
    /// </summary>
    public SourceCode Code
    {
        get => _Code;
        protected set
        {
            if (SetProperty(ref _Code, value))
            {
                OnCodeChanged(value);
            }
        }
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

            OnTextChanged(value);
        }
    }

    private bool _IsUnsavedEditPending;

    /// <summary>
    /// Gets whether or not there are pending unsaved changes to the current file
    /// </summary>
    public bool IsUnsavedEditPending
    {
        get => _IsUnsavedEditPending;
        private set => SetProperty(ref _IsUnsavedEditPending, value);
    }

    private SyntaxValidationResult _ValidationResult;

    /// <summary>
    /// Gets the current <see cref="SyntaxValidationResult"/> value for <see cref="Text"/>
    /// </summary>
    public SyntaxValidationResult ValidationResult
    {
        get => _ValidationResult;
        set => SetProperty(ref _ValidationResult, value);
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
            Guard.IsGreaterThan(value, 0);

            SetProperty(ref _Row, value);
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
            Guard.IsGreaterThan(value, 0);

            SetProperty(ref _Column, value);
        }
    }

    /// <summary>
    /// Raised whenever <see cref="Code"/> changes
    /// </summary>
    /// <param name="code">Thew value for <see cref="Code"/></param>
    protected virtual void OnCodeChanged(SourceCode code) { }

    /// <summary>
    /// Raised whenever <see cref="Text"/> changes
    /// </summary>
    /// <param name="text">The new value for <see cref="Text"/></param>
    protected virtual void OnTextChanged(ReadOnlyMemory<char> text) { }

    /// <summary>
    /// Reports that <see cref="Code"/> has been saved, and updates <see cref="IsUnsavedEditPending"/>
    /// </summary>
    protected void ReportCodeSaved()
    {
        IsUnsavedEditPending = !Text.Span.SequenceEqual(Code.Content.AsSpan());
    }
}
