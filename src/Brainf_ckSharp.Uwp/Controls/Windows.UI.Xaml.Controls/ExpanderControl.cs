﻿using Microsoft.Toolkit.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Windows.UI.Xaml.Controls;

/// <summary>
/// A custom <see cref="Control"/> that displays a header and an expandable content
/// </summary>
[TemplatePart(Name = ExpanderButtonName, Type = typeof(Button))]
public sealed class ExpanderControl : Control
{
    // Constants for the template
    private const string ExpanderButtonName = "ExpanderButton";
    private const string CollapsedVisualStateName = "Collapsed";
    private const string ExpandedVisualStateName = "Expanded";

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        Button expanderButton = (Button?)GetTemplateChild(ExpanderButtonName)
                                ?? ThrowHelper.ThrowInvalidOperationException<Button>("Can't find " + ExpanderButtonName);

        expanderButton.Click += (s, e) => IsExpanded = !IsExpanded;

        if (IsExpanded) VisualStateManager.GoToState(this, ExpandedVisualStateName, false);
        else VisualStateManager.GoToState(this, CollapsedVisualStateName, false);
    }

    /// <summary>
    /// Gets or sets the header content
    /// </summary>
    public FrameworkElement Header
    {
        get => (FrameworkElement)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// The dependency property for <see cref="Header"/>
    /// </summary>
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(Header),
        typeof(FrameworkElement),
        typeof(ExpanderControl),
        new(default(FrameworkElement)));

    /// <summary>
    /// Gets or sets the height of the header content
    /// </summary>
    public double HeaderHeight
    {
        get => (double)GetValue(HeaderHeightProperty);
        set => SetValue(HeaderHeightProperty, value);
    }

    /// <summary>
    /// The dependency property for <see cref="HeaderHeight"/>
    /// </summary>
    public static readonly DependencyProperty HeaderHeightProperty = DependencyProperty.Register(
        nameof(HeaderHeight),
        typeof(double),
        typeof(ExpanderControl),
        new(default(double)));

    /// <summary>
    /// Gets or sets the expandable content
    /// </summary>
    public FrameworkElement ExpandableContent
    {
        get => (FrameworkElement)GetValue(ExpandableContentProperty);
        set => SetValue(ExpandableContentProperty, value);
    }

    /// <summary>
    /// The dependency property for <see cref="ExpandableContent"/>
    /// </summary>
    public static readonly DependencyProperty ExpandableContentProperty = DependencyProperty.Register(
        nameof(ExpandableContent),
        typeof(FrameworkElement),
        typeof(ExpanderControl),
        new(default(FrameworkElement)));

    /// <summary>
    /// Gets or sets the height of the header content
    /// </summary>
    public double ExpandableContentHeight
    {
        get => (double)GetValue(ExpandableContentHeightProperty);
        set => SetValue(ExpandableContentHeightProperty, value);
    }

    /// <summary>
    /// The dependency property for <see cref="ExpandableContentHeight"/>
    /// </summary>
    public static readonly DependencyProperty ExpandableContentHeightProperty = DependencyProperty.Register(
        nameof(ExpandableContentHeight),
        typeof(double),
        typeof(ExpanderControl),
        new(default(double)));

    /// <summary>
    /// Gets or sets whether or not the control is currently expanded
    /// </summary>
    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    /// <summary>
    /// The dependency property for <see cref="IsExpanded"/>
    /// </summary>
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
        nameof(IsExpanded),
        typeof(bool),
        typeof(ExpanderControl),
        new(true, OnIsExpandedPropertyChanged));

    /// <summary>
    /// Updates the UI when <see cref="IsExpanded"/> changes
    /// </summary>
    /// <param name="d">The source <see cref="DependencyObject"/> instance</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> info for the current update</param>
    private static void OnIsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExpanderControl @this = (ExpanderControl)d;
        bool value = (bool)e.NewValue;

        if (value) VisualStateManager.GoToState(@this, ExpandedVisualStateName, true);
        else VisualStateManager.GoToState(@this, CollapsedVisualStateName, true);
    }
}
