using Windows.Devices.Input;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using UICompositionAnimations.Lights;

namespace Brainf_ck_sharp_UWP.UserControls.InheritedControls
{
    /// <summary>
    /// A simple <see cref="Grid"/> control that automatically handles two lights to host reveal highlight effects
    /// </summary>
    public sealed class LightsContainerGrid : Grid
    {
        // Indicates whether or not the lights are currently enabled
        private bool _LightsEnabled;

        public LightsContainerGrid()
        {
            // Lights setup
            PointerPositionSpotLight
                light = new PointerPositionSpotLight { Active = false },
                wideLight = new PointerPositionSpotLight
                {
                    IdAppendage = "[Wide]",
                    Z = 30,
                    Shade = 0x10,
                    Active = false
                };
            Lights.Add(light);
            Lights.Add(wideLight);

            // Animate the lights when the pointer exits and leaves the area
            this.ManageHostPointerStates((type, value) =>
            {
                bool lightsVisible = type == PointerDeviceType.Mouse && value;
                if (_LightsEnabled == lightsVisible) return;
                light.Active = wideLight.Active = _LightsEnabled = lightsVisible;
            });
        }
    }
}
