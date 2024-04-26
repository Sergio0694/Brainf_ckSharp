using System.Runtime.InteropServices;

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
    /// Gets the current CPU architecture for the application package
    /// </summary>
    Architecture CpuArchitecture { get; }

    /// <summary>
    /// Gets the current operating system
    /// </summary>
    string OperatingSystem { get; }

    /// <summary>
    /// Gets the current version of the operating system
    /// </summary>
    string OperatingSystemVersion { get; }
}
