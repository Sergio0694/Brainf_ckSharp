using Brainf_ckSharp.Services.Enums;

namespace Brainf_ckSharp.Services;

/// <summary>
/// The default <see langword="interface"/> for the a service that reports system information
/// </summary>
public interface ISystemInformationService
{
    /// <summary>
    /// Gets the current application version
    /// </summary>
    string ApplicationVersion { get; }

    /// <summary>
    /// Gets the current CPU architecture
    /// </summary>
    CpuArchitecture CpuArchitecture { get; }

    /// <summary>
    /// Gets the current operating system
    /// </summary>
    string OperatingSystem { get; }

    /// <summary>
    /// Gets the current version of the operating system
    /// </summary>
    string OperatingSystemVersion { get; }
}
