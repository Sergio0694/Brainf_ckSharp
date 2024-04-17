using System;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections.Abstract;

/// <summary>
/// A base class for a viewmodel representing a settings section
/// </summary>
public abstract class SettingsSectionViewModelBase : ObservableRecipient
{
    /// <summary>
    /// The <see cref="ISettingsService"/> instance currently in use
    /// </summary>
    protected readonly ISettingsService SettingsService;

    /// <summary>
    /// Creates a new <see cref="SettingsSubPageViewModel"/> instance
    /// </summary>
    /// <param name="messenger">The <see cref="IMessenger"/> instance to use</param>
    /// <param name="settingsService">The <see cref="ISettingsService"/> instance to use</param>
    protected SettingsSectionViewModelBase(IMessenger messenger, ISettingsService settingsService)
        : base(messenger)
    {
        this.SettingsService = settingsService;
    }

    /// <summary>
    /// A proxy for <see cref="ObservableObject.SetProperty{T}(ref T, T, string)"/> that
    /// also overwrites the value stored in the local settings when a property changes
    /// </summary>
    /// <typeparam name="T">The type of setting to set</typeparam>
    /// <param name="field">The previous setting value</param>
    /// <param name="value">The new value to set</param>
    /// <param name="name">The name of the setting that changed</param>
    protected new void SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (base.SetProperty(ref field, value, name))
        {
            this.SettingsService.SetValue(name!, value);
        }
    }

    /// <summary>
    /// A proxy for <see cref="ObservableObject.SetProperty{T}(ref T, T, string)"/> that
    /// also overwrites the value stored in the local settings when a property changes and broadcasts a message
    /// </summary>
    /// <typeparam name="T">The type of setting to set</typeparam>
    /// <typeparam name="TMessage">The type of message to broadcast</typeparam>
    /// <param name="field">The previous setting value</param>
    /// <param name="value">The new value to set</param>
    /// <param name="name">The name of the setting that changed</param>
    protected bool SetProperty<T, TMessage>(ref T field, T value, [CallerMemberName] string? name = null)
        where TMessage : ValueChangedMessage<T>
    {
        if (base.SetProperty(ref field, value, name))
        {
            this.SettingsService.SetValue(name!, value);

            TMessage message = (TMessage)Activator.CreateInstance(typeof(TMessage), value);

            _ = Messenger.Send(message);

            return true;
        }

        return false;
    }
}
