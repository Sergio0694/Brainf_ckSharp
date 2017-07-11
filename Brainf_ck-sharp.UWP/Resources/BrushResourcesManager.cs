using Windows.UI;
using Windows.UI.Xaml.Media;
using Brainf_ck_sharp_UWP.Helpers;
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
            AccentBrush = XAMLResourcesHelper.GetResourceValue<SolidColorBrush>("SystemControlHighlightAccentBrush");
            AccentBrushLowFadeBrush = new SolidColorBrush(Color.FromArgb(0x90, AccentBrush.Color.R, AccentBrush.Color.G, AccentBrush.Color.B));
            XAMLResourcesHelper.AssignValueToXAMLResource("AccentBrushLowFade", AccentBrushLowFadeBrush);
            AccentBrushMediumFadeBrush = new SolidColorBrush(Color.FromArgb(0xB0, AccentBrush.Color.R, AccentBrush.Color.G, AccentBrush.Color.B));
            XAMLResourcesHelper.AssignValueToXAMLResource("AccentBrushMediumFade", AccentBrushMediumFadeBrush);
            RedDangerBrush = XAMLResourcesHelper.GetResourceValue<SolidColorBrush>("RedDangerBrush");
            XAMLResourcesHelper.AssignValueToXAMLResource("SubMenuFlyoutPointerOverBrush", new SolidColorBrush(Color.FromArgb(0x70, AccentBrush.Color.R, AccentBrush.Color.G, AccentBrush.Color.B)));
            XAMLResourcesHelper.AssignValueToXAMLResource("SubMenuFlyoutOpenedBrush", new SolidColorBrush(Color.FromArgb(0x50, AccentBrush.Color.R, AccentBrush.Color.G, AccentBrush.Color.B)));
            XamlLight.AddTargetBrush($"{PointerPositionSpotLight.GetIdStatic()}[Popup]", XAMLResourcesHelper.GetResourceValue<LightingBrush>("PopupElementsWideLightBrush"));
            XamlLight.AddTargetBrush($"{PointerPositionSpotLight.GetIdStatic()}[Wide]", XAMLResourcesHelper.GetResourceValue<LightingBrush>("ElementsWideLightBrush"));
        }

        #region Brushes

        /// <summary>
        /// Gets the default accent brush (SystemControlHighlightAccentBrush)
        /// </summary>
        public SolidColorBrush AccentBrush { get; }

        /// <summary>
        /// Gets the accent brush with 0x90 as the alpha channel
        /// </summary>
        public SolidColorBrush AccentBrushLowFadeBrush { get; }

        /// <summary>
        /// Gets the accent brush with 0xB0 as the alpha channel
        /// </summary>
        public SolidColorBrush AccentBrushMediumFadeBrush { get; }

        /// <summary>
        /// Gets the RedDangerBrush brush (#FFDC232B)
        /// </summary>
        public SolidColorBrush RedDangerBrush { get; }

        /// <summary>
        /// Gets the dark brush that acts as a shade background behind the elements wide light brush
        /// </summary>
        public SolidColorBrush WideLightBrushDarkShadeBackground { get; } = XAMLResourcesHelper.GetResourceValue<SolidColorBrush>("WideLightBrushDarkShadeBackground");

        #endregion
    }
}
