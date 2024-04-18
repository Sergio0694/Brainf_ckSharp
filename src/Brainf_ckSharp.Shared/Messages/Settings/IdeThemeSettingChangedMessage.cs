using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Enums.Settings;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Shared.Messages.Settings;

/// <summary>
/// A messsage that signals whenever the <see cref="IdeTheme"/> value for the <see cref="SettingsKeys.IdeTheme"/> setting changes
/// </summary>
/// <param name="value">The new setting value</param>
public sealed class IdeThemeSettingChangedMessage(IdeTheme value) : ValueChangedMessage<IdeTheme>(value);
