﻿using CommunityToolkit.Diagnostics;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Host.InputPanel;

/// <summary>
/// A templated <see cref="Control"/> that acts as a minimal header button in the stdin header
/// </summary>
[TemplatePart(Name = RootButtonName, Type = typeof(Button))]
public sealed class MinimalHeaderButton : Control
{
    // Constants for the template
    private const string RootButtonName = "RootButton";
    private const string DisabledVisualStateName = "Disabled";
    private const string DefaultVisualStateName = "Default";
    private const string SelectedVisualStateName = "Selected";

    /// <summary>
    /// The root <see cref="Button"/> control
    /// </summary>
    private Button? _RootButton;

    /// <summary>
    /// Creates a new <see cref="MinimalHeaderButton"/> instance
    /// </summary>
    public MinimalHeaderButton()
    {
        RegisterPropertyChangedCallback(IsEnabledProperty, OnIsEnabledChanged);
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        this._RootButton = (Button?)GetTemplateChild(RootButtonName)
                      ?? ThrowHelper.ThrowInvalidOperationException<Button>("Can't find " + RootButtonName);

        this._RootButton.Click += RootButton_Click;

        UpdateVisualState();
    }

    /// <summary>
    /// Gets or sets the icon for the current control
    /// </summary>
    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// The dependency property for <see cref="Icon"/>
    /// </summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(string),
        typeof(MinimalHeaderButton),
        new(DependencyProperty.UnsetValue));

    /// <summary>
    /// Gets or sets whether or not the control is currently selected
    /// </summary>
    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    /// <summary>
    /// The dependency property for <see cref="IsSelected"/>
    /// </summary>
    public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
        nameof(IsSelected),
        typeof(bool),
        typeof(MinimalHeaderButton),
        new(DependencyProperty.UnsetValue, OnIsSelectedPropertyChanged));

    /// <summary>
    /// Raised whenever the <see cref="IsSelected"/> property is set to <see langword="true"/>
    /// </summary>
    public event EventHandler? Selected;

    /// <summary>
    /// Raised whenever the <see cref="IsSelected"/> property is set to <see langword="false"/>
    /// </summary>
    public event EventHandler? Deselected;

    /// <summary>
    /// Updates the UI when <see cref="IsSelected"/> changes
    /// </summary>
    /// <param name="d">The source <see cref="DependencyObject"/> instance</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> info for the current update</param>
    private static void OnIsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        MinimalHeaderButton @this = (MinimalHeaderButton)d;

        if (e.NewValue is bool value && value)
        {
            @this.Selected?.Invoke(@this, EventArgs.Empty);
        }
        else
        {
            @this.Deselected?.Invoke(@this, EventArgs.Empty);
        }

        @this.UpdateVisualState();
    }

    /// <summary>
    /// Applies the correct visual state when the <see cref="IsSelected"/> or <see cref="Control.IsEnabled"/> property change
    /// </summary>
    /// <remarks>This method also needs to be called when the template is applied</remarks>
    private void UpdateVisualState()
    {
        if (!IsEnabled)
        {
            VisualStateManager.GoToState(this, DisabledVisualStateName, false);
        }
        else if (IsSelected)
        {
            VisualStateManager.GoToState(this, SelectedVisualStateName, false);
        }
        else
        {
            VisualStateManager.GoToState(this, DefaultVisualStateName, false);
        }
    }

    /// <summary>
    /// Updates the UI when <see cref="Control.IsEnabled"/> changes
    /// </summary>
    /// <param name="sender">The source <see cref="DependencyObject"/> instance</param>
    /// <param name="dp">The source <see cref="DependencyProperty"/> info for the current update</param>
    private void OnIsEnabledChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (!IsEnabled && IsSelected)
        {
            IsSelected = false;
        }
        else
        {
            UpdateVisualState();
        }
    }

    // Updates the UI when the control is selected
    private void RootButton_Click(object sender, RoutedEventArgs e)
    {
        IsSelected = true;
    }
}
