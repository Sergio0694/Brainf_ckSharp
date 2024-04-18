using System;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Shared.Models.Ide;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

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

    private SourceCode code = SourceCode.CreateEmpty();

    /// <summary>
    /// Gets or sets the loaded source code
    /// </summary>
    public SourceCode Code
    {
        get => this.code;
        protected set
        {
            if (SetProperty(ref this.code, value))
            {
                OnCodeChanged(value);
            }
        }
    }

    private ReadOnlyMemory<char> text = SourceCode.EmptyContent.AsMemory();

    /// <summary>
    /// Gets or sets the currently displayed text
    /// </summary>
    public ReadOnlyMemory<char> Text
    {
        get => this.text;
        set
        {
            if (this.text.Span.SequenceEqual(value.Span))
            {
                return;
            }

            this.text = value;

            OnPropertyChanged();

            IsUnsavedEditPending = !value.Span.SequenceEqual(Code.Content.AsSpan());

            OnTextChanged(value);
        }
    }

    private bool isUnsavedEditPending;

    /// <summary>
    /// Gets whether or not there are pending unsaved changes to the current file
    /// </summary>
    public bool IsUnsavedEditPending
    {
        get => this.isUnsavedEditPending;
        private set => SetProperty(ref this.isUnsavedEditPending, value);
    }

    private SyntaxValidationResult validationResult;

    /// <summary>
    /// Gets the current <see cref="SyntaxValidationResult"/> value for <see cref="Text"/>
    /// </summary>
    public SyntaxValidationResult ValidationResult
    {
        get => this.validationResult;
        set => SetProperty(ref this.validationResult, value);
    }

    private int row = 1;

    /// <summary>
    /// Gets the current row in the document in use
    /// </summary>
    public int Row
    {
        get => this.row;
        set
        {
            Guard.IsGreaterThan(value, 0);

            _ = SetProperty(ref this.row, value);
        }
    }

    private int column = 1;

    /// <summary>
    /// Gets the current column in the document in use
    /// </summary>
    public int Column
    {
        get => this.column;
        set
        {
            Guard.IsGreaterThan(value, 0);

            _ = SetProperty(ref this.column, value);
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
