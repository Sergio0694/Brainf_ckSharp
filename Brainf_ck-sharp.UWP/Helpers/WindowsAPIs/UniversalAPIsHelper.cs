using System;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;

namespace Brainf_ck_sharp_UWP.Helpers.WindowsAPIs
{
    /// <summary>
    /// A static class that helps checking whether or not a specific runtime Type is present on the current device
    /// </summary>
    public static class UniversalAPIsHelper
    {
        /// <summary>
        /// Checks if the given Type is present on the device
        /// </summary>
        /// <param name="type">The runtime Type to check</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTypePresent(this Type type) => IsTypePresent(type.FullName);

        /// <summary>
        /// Checks if the given Type is present on the device
        /// </summary>
        /// <param name="typeFullName">The full name of the Type to check</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTypePresent(String typeFullName) => ApiInformation.IsTypePresent(typeFullName);

        private static bool? _IsMobileDevice;

        /// <summary>
        /// Gets whether or not the device is a mobile phone
        /// </summary>
        public static bool IsMobileDevice
        {
            get
            {
                if (_IsMobileDevice == null)
                {
                    try
                    {
                        IObservableMap<String, String> qualifiers = ResourceContext.GetForCurrentView().QualifierValues;
                        _IsMobileDevice = qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"] == "Mobile";
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // No idea why this should happen
                        return IsTypePresent(typeof(Windows.Phone.UI.Input.HardwareButtons));
                    }
                }
                return _IsMobileDevice.Value;
            }
        }

        private static bool? _IsDesktop;

        /// <summary>
        /// Gets whether or not the device is running Windows 10 Desktop
        /// </summary>
        public static bool IsDesktop
        {
            get
            {
                if (_IsDesktop == null)
                {
                    try
                    {
                        IObservableMap<String, String> qualifiers = ResourceContext.GetForCurrentView().QualifierValues;
                        _IsDesktop = qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"] == "Desktop";
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Weird crash, but still...
                        return false;
                    }
                }
                return _IsDesktop.Value;
            }
        }
    }
}
