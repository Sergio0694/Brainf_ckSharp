using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Brainf_ck_sharp_UWP.Helpers;

namespace Brainf_ck_sharp_UWP.Resources
{
    public class BrushResourcesManager
    {
        public static BrushResourcesManager Instance { get; private set; }

        public static void InitializeOrRefreshInstance() => Instance = new BrushResourcesManager();

        private BrushResourcesManager()
        {
            AccentBrush = StaticHelper.GetResourceValue<SolidColorBrush>("SystemControlHighlightAccentBrush");
            AccentBrushLowFadeBrush = new SolidColorBrush(Color.FromArgb(0x90, AccentBrush.Color.R, AccentBrush.Color.G, AccentBrush.Color.B));
            StaticHelper.AssignValueToXAMLResource("AccentBrushLowFade", AccentBrushLowFadeBrush);
            AccentBrushMediumFadeBrush = new SolidColorBrush(Color.FromArgb(0xB0, AccentBrush.Color.R, AccentBrush.Color.G, AccentBrush.Color.B));
            StaticHelper.AssignValueToXAMLResource("AccentBrushMediumFade", AccentBrushMediumFadeBrush);
        }

        #region Brushes

        public SolidColorBrush AccentBrush { get; }

        public SolidColorBrush AccentBrushLowFadeBrush { get; }

        public SolidColorBrush AccentBrushMediumFadeBrush { get; }

        #endregion
    }
}
