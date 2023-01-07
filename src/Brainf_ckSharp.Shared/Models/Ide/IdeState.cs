using System.Text.Json.Serialization;

namespace Brainf_ckSharp.Shared.Models.Ide;

/// <summary>
/// A model representing a workspace state to be serialized and deserialized.
/// </summary>
/// <param name="Text">Gets the text currently displayed.</param>
/// <param name="Row">Gets the current row in the document in use.</param>
/// <param name="Column">Gets the current column in the document in use.</param>
/// <param name="FilePath">Gets the oath of the file in use, if present.</param>
public sealed record IdeState(
    [property: JsonRequired] string Text,
    [property: JsonRequired] int Row,
    [property: JsonRequired] int Column,
    string? FilePath);
