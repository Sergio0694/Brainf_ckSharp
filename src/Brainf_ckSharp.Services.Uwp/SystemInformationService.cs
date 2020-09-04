using Windows.System;
using Brainf_ckSharp.Services.Enums;
using Microsoft.Toolkit.Uwp.Helpers;
using UwpInfo = Microsoft.Toolkit.Uwp.Helpers.SystemInformation;

#nullable enable

namespace Brainf_ckSharp.Services.Uwp.SystemInformation
{
    /// <summary>
    /// A <see langword="class"/> that reports system information
    /// </summary>
    public sealed class SystemInformationService : ISystemInformationService
    {
        /// <summary>
        /// Creates a new <see cref="SystemInformationService"/> instance
        /// </summary>
        public SystemInformationService()
        {
            ApplicationVersion = UwpInfo.Instance.ApplicationVersion.ToFormattedString();
            CpuArchitecture = UwpInfo.Instance.OperatingSystemArchitecture switch
            {
                ProcessorArchitecture.X86 => CpuArchitecture.X86,
                ProcessorArchitecture.X64 => CpuArchitecture.X64,
                ProcessorArchitecture.Arm => CpuArchitecture.Arm,
                ProcessorArchitecture.Arm64 => CpuArchitecture.Arm64,
                _ => CpuArchitecture.Unknown
            };
            OperatingSystem = UwpInfo.Instance.OperatingSystem;
            OperatingSystemVersion = UwpInfo.Instance.OperatingSystemVersion.ToString();
        }

        /// <inheritdoc/>
        public string ApplicationVersion { get; }

        /// <inheritdoc/>
        public CpuArchitecture CpuArchitecture { get; }

        /// <inheritdoc/>
        public string OperatingSystem { get; }

        /// <inheritdoc/>
        public string OperatingSystemVersion { get; }
    }
}
