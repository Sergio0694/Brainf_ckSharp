using Windows.UI;
using Windows.UI.Xaml.Media;
using Brainf_ck_sharp_UWP.Helpers.UI;
using UICompositionAnimations.Brushes;
using UICompositionAnimations.Lights;

namespace Brainf_ck_sharp_UWP.Resources
{
    /// <summary>
    /// A simple class to manage the app custom brushes
    /// </summary>
    public class BrushResourcesManager
    {
        /// <summary>
        /// Gets the current instance with the initializes brushes to use
        /// </summary>
        public static BrushResourcesManager Instance { get; private set; }

        /// <summary>
        /// Initializes the shared instance or refreshes it with the updated brushes (if the user changed the accent color)
        /// </summary>
        public static void InitializeOrRefreshInstance() => Instance = new BrushResourcesManager();

        // Initializes the brushes
        private BrushResourcesManager()
        {
            SolidColorBrush accent = XAMLResourcesHelper.GetResourceValue<SolidColorBrush>("SystemControlHighlightAccentBrush");
            XAMLResourcesHelper.SetResourceValue("AccentBrushLowFade", new SolidColorBrush(Color.FromArgb(0x90, accent.Color.R, accent.Color.G, accent.Color.B)));
            XAMLResourcesHelper.SetResourceValue("AccentBrushMediumFade", new SolidColorBrush(Color.FromArgb(0xB0, accent.Color.R, accent.Color.G, accent.Color.B)));
            XAMLResourcesHelper.SetResourceValue("SubMenuFlyoutPointerOverBrush", new SolidColorBrush(Color.FromArgb(0x70, accent.Color.R, accent.Color.G, accent.Color.B)));
            XAMLResourcesHelper.SetResourceValue("SubMenuFlyoutOpenedBrush", new SolidColorBrush(Color.FromArgb(0x50, accent.Color.R, accent.Color.G, accent.Color.B)));

            // Lights
            LightingBrush
                bb = new LightingBrush(),
                bwb = new LightingBrush();
            PointerPositionSpotLight.SetIsTarget(bb, true);
            XamlLight.AddTargetBrush($"{PointerPositionSpotLight.GetIdStatic()}[Wide]", bwb);
            XAMLResourcesHelper.SetResourceValue("BorderLightBrush", bb, true);
            XAMLResourcesHelper.SetResourceValue("ElementsWideLightBrush", bwb, true);
        }
    }
}
