using Windows.UI;
using Windows.UI.Xaml.Media;
using Brainf_ck_sharp_UWP.Helpers;
using UICompositionAnimations.Brushes;
using UICompositionAnimations.Helpers;
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
            XAMLResourcesHelper.SetResourceValue("AccentBrushLowFade", AccentBrushLowFadeBrush);
            AccentBrushMediumFadeBrush = new SolidColorBrush(Color.FromArgb(0xB0, AccentBrush.Color.R, AccentBrush.Color.G, AccentBrush.Color.B));
            XAMLResourcesHelper.SetResourceValue("AccentBrushMediumFade", AccentBrushMediumFadeBrush);
            RedDangerBrush = XAMLResourcesHelper.GetResourceValue<SolidColorBrush>("RedDangerBrush");
            XAMLResourcesHelper.SetResourceValue("SubMenuFlyoutPointerOverBrush", new SolidColorBrush(Color.FromArgb(0x70, AccentBrush.Color.R, AccentBrush.Color.G, AccentBrush.Color.B)));
            XAMLResourcesHelper.SetResourceValue("SubMenuFlyoutOpenedBrush", new SolidColorBrush(Color.FromArgb(0x50, AccentBrush.Color.R, AccentBrush.Color.G, AccentBrush.Color.B)));

            // Lights
            if (ApiInformationHelper.IsMobileDevice)
            {
                SolidColorBrush t = new SolidColorBrush { Color = Colors.Transparent };
                XAMLResourcesHelper.SetResourceValue("BorderLightBrush", t);
                XAMLResourcesHelper.SetResourceValue("PopupElementsLightBrush", t);
                XAMLResourcesHelper.SetResourceValue("ElementsWideLightBrush", t);
                XAMLResourcesHelper.SetResourceValue("PopupElementsWideLightBrush", t);
                XAMLResourcesHelper.SetResourceValue("WideLightBrushDarkShadeBackground", t);
            }
            else
            {
                LightingBrush
                    bb = new LightingBrush(),
                    pbb = new LightingBrush(),
                    bwb = new LightingBrush(),
                    pbwb = new LightingBrush();
                PointerPositionSpotLight.SetIsTarget(bb, true);
                PointerPositionSpotLight.SetIsTarget(pbb, true);
                XamlLight.AddTargetBrush($"{PointerPositionSpotLight.GetIdStatic()}[Wide]", bwb);
                XamlLight.AddTargetBrush($"{PointerPositionSpotLight.GetIdStatic()}[Popup]", pbwb);
                XAMLResourcesHelper.SetResourceValue("BorderLightBrush", bb);
                XAMLResourcesHelper.SetResourceValue("PopupElementsLightBrush", pbb);
                XAMLResourcesHelper.SetResourceValue("ElementsWideLightBrush", bwb);
                XAMLResourcesHelper.SetResourceValue("PopupElementsWideLightBrush", pbwb);
                SolidColorBrush sb = new SolidColorBrush { Color = Color.FromArgb(0x10, 0, 0, 0), Opacity = 0 };
                XAMLResourcesHelper.SetResourceValue("WideLightBrushDarkShadeBackground", sb);
                WideLightBrushDarkShadeBackground = sb;
            }
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
        public SolidColorBrush WideLightBrushDarkShadeBackground { get; }

        #endregion
    }
}
