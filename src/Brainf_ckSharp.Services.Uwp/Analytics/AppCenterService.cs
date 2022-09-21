using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Microsoft.Toolkit.Diagnostics;

#nullable enable

namespace Brainf_ckSharp.Services.Uwp.Analytics
{
    /// <summary>
    /// An <see cref="IAnalyticsService"/> implementation using AppCenter
    /// </summary>
    public sealed class AppCenterService : IAnalyticsService
    {
        /// <summary>
        /// Indicates whether or not <see cref="Initialize"/> has already been called
        /// </summary>
        private int _IsInitialized;

        /// <inheritdoc/>
        public void Initialize(string secret)
        {
            if (Interlocked.CompareExchange(ref _IsInitialized, 1, 0) != 0)
            {
                ThrowHelper.ThrowInvalidOperationException("The service has already been initialized");
            }

            AppCenter.Start(secret, typeof(Crashes), typeof(Microsoft.AppCenter.Analytics.Analytics));
        }

        /// <summary>
        /// The maximum length for any property name and value
        /// </summary>
        private const int PropertyStringMaxLength = 124; // It's 125, but one character is reserved for the leading '|' to indicate trimming

        /// <inheritdoc/>
        public void Log(string title, params (string Property, string Value)[]? data)
        {
            IDictionary<string, string>? properties = data?.ToDictionary(
                pair => pair.Property,
                pair => pair.Value.Length <= PropertyStringMaxLength
                    ? pair.Value
                    : $"|{pair.Value.Substring(pair.Value.Length - PropertyStringMaxLength)}");

            Microsoft.AppCenter.Analytics.Analytics.TrackEvent(title, properties);
        }
    }
}
