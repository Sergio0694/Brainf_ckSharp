#nullable enable

namespace Brainf_ckSharp.Services.Uwp.Analytics;

/// <summary>
/// A <see langword="class"/> that manages the analytics service in a release environment
/// </summary>
public sealed class ReleaseAnalyticsService : IAnalyticsService
{
    /// <inheritdoc/>
    public void Initialize(string secret)
    {
    }

    /// <inheritdoc/>
    public void Log(string title, params (string Property, string Value)[]? data)
    {
    }
}
