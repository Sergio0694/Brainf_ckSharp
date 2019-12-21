using System;

namespace Brainf_ck_sharp_UWP.Helpers.Settings
{
    /// <summary>
    /// A simple <see langword="class"/> that reads and converts some common app settings
    /// </summary>
    public static class AppSettingsParser
    {
        /// <summary>
        /// Gets the memory size corresponding to the current <see cref="AppSettingsKeys.InterpreterMemorySize"/> setting
        /// </summary>
        public static int InterpreterMemorySize
        {
            get
            {
                int selection = AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.InterpreterMemorySize));
                switch (selection)
                {
                    case 0: return 32;
                    case 1: return 48;
                    case 2: return 64;
                    default: throw new ArgumentOutOfRangeException(nameof(AppSettingsKeys.InterpreterMemorySize), "Invalid memory size setting");
                }
            }
        }
    }
}
