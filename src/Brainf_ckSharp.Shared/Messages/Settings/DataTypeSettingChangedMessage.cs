using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Shared.Constants;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Shared.Messages.Settings;

/// <summary>
/// A messsage that signals whenever the <see cref="DataType"/> value for the <see cref="SettingsKeys.DataType"/> setting changes
/// </summary>
/// <param name="value">The new setting value</param>
public sealed class DataTypeSettingChangedMessage(DataType value) : ValueChangedMessage<DataType>(value);
