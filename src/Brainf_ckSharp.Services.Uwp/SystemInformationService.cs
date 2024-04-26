using System;
using System.Runtime.InteropServices;
using Windows.ApplicationModel;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System;
using Windows.System.Profile;

#nullable enable

namespace Brainf_ckSharp.Services.Uwp.SystemInformation;

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
        PackageVersion packageVersion = Package.Current.Id.Version;

        Version packageVersion2 = new(
            packageVersion.Major,
            packageVersion.Minor,
            packageVersion.Build,
            packageVersion.Revision);

        ApplicationVersion = packageVersion2.ToString();

        CpuArchitecture = Package.Current.Id.Architecture switch
        {
            ProcessorArchitecture.X86 => Architecture.X86,
            ProcessorArchitecture.X64 => Architecture.X64,
            ProcessorArchitecture.Arm => Architecture.Arm,
            ProcessorArchitecture.Arm64 => Architecture.Arm64,
            _ => (Architecture)int.MaxValue
        };

        OperatingSystem = new EasClientDeviceInformation().OperatingSystem;

        ulong version = ulong.Parse(AnalyticsInfo.VersionInfo.DeviceFamilyVersion);

        Version operatingSystemVersion = new(
            (ushort)((version & 0xFFFF000000000000L) >> 48),
            (ushort)((version & 0x0000FFFF00000000L) >> 32),
            (ushort)((version & 0x00000000FFFF0000L) >> 16),
            (ushort)(version & 0x000000000000FFFFL));

        OperatingSystemVersion = operatingSystemVersion.ToString();
    }

    /// <inheritdoc/>
    public string ApplicationVersion { get; }

    /// <inheritdoc/>
    public Architecture CpuArchitecture { get; }

    /// <inheritdoc/>
    public string OperatingSystem { get; }

    /// <inheritdoc/>
    public string OperatingSystemVersion { get; }
}
