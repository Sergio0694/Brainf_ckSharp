using System.Diagnostics;
using System.Text;

#nullable enable

namespace Brainf_ckSharp.Services.Uwp.Analytics;

/// <summary>
/// A <see langword="class"/> that manages the analytics service in a test environment
/// </summary>
public sealed class TestAnalyticsService : IAnalyticsService
{
    /// <inheritdoc/>
    public void Initialize(string secret) { }

    /// <inheritdoc/>
    public void Log(string title, params (string Property, string Value)[]? data)
    {
        StringBuilder builder = new();

        builder.AppendLine($"[EVENT]: \"{title}\"");

        if (data is not null)
        {
            foreach ((string Property, string Value) info in data)
            {
                builder.AppendLine($">> {info.Property}: \"{info.Value}\"");
            }
        }

        Debug.Write(builder);
    }
}
