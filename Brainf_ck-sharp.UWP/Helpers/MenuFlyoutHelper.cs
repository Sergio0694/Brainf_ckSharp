using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Enums;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Helpers
{
    /// <summary>
    /// A static class that prepares <see cref="MenuFlyout"/> objects to display
    /// </summary>
    public static class MenuFlyoutHelper
    {
        #region Tools

        // The minimum menu item width
        private static readonly double FlyoutMinWidth = XAMLResourcesHelper.GetResourceValue<double>("MenuFlyoutMinWidth");

        // Adds a new item to the target menu flyout
        private static void AddItem([NotNull] this ICollection<MenuFlyoutItemBase> items, [NotNull] String text, [NotNull] String tag, [NotNull] Action click)
        {
            // Setup the new item to add
            MenuFlyoutItem menuItem = new MenuFlyoutItem
            {
                Text = text,
                Tag = tag,
                Style = XAMLResourcesHelper.GetResourceValue<Style>("MenuFlyoutItemIconTemplate")
            };
            AddItem(items, menuItem, click);
        }

        // Adds an existing item to the target menu flyout
        private static void AddItem([NotNull] this ICollection<MenuFlyoutItemBase> items, [NotNull] MenuFlyoutItem menuItem, [NotNull] Action click)
        {
            // Adjust the width of the target element
            menuItem.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            if (menuItem.DesiredSize.Width + 12 > FlyoutMinWidth)
            {
                menuItem.MinWidth = menuItem.DesiredSize.Width + 12;
            }

            // Add the click handler if needed and add the item to the target collection
            menuItem.Click += (s, e) => click();
            items.Add(menuItem);
        }

        // Adds an horizontal separator
        private static void AddSeparator(this MenuFlyout menu) => menu.Items?.Add(new MenuFlyoutSeparator());

        #endregion

        /// <summary>
        /// Prepares the <see cref="MenuFlyout"/> for the saved source codes
        /// </summary>
        /// <param name="favorite">The action when the selected item is favorited/unfavorited</param>
        /// <param name="favorited">Indicates the favorited state for the current item</param>
        /// <param name="rename">The action when the item is renamed</param>
        /// <param name="share">The action when the users selects a share method for the item</param>
        /// <param name="delete">The action when the user requests to delete the item</param>
        public static MenuFlyout PrepareSavedSourceCodeMenuFlyout(
            [NotNull] Action favorite, bool favorited, 
            [NotNull] Action rename,
            [NotNull] Action<SourceCodeShareType> share,
            [NotNull] Action delete)
        {
            MenuFlyout menu = new MenuFlyout();
            menu.Items?.AddItem(LocalizationManager.GetResource(favorited ? "Unfavorite" : "Favorite"),
                (favorited ? 0xE195 : 0xE249).ToSegoeMDL2Icon(), favorite);
            menu.Items?.AddItem(LocalizationManager.GetResource("Rename"), 0xE104.ToSegoeMDL2Icon(), rename);
            MenuFlyoutSubItem sub = new MenuFlyoutSubItem { Text = LocalizationManager.GetResource("Share") };
            sub.Items?.AddItem(LocalizationManager.GetResource("Clipboard"), 0xEF20.ToSegoeMDL2Icon(), () => share(SourceCodeShareType.Clipboard));
            sub.Items?.AddItem(LocalizationManager.GetResource("OSShare"), 0xED4D.ToSegoeMDL2Icon(), () => share(SourceCodeShareType.OSShare));
            sub.Items?.AddItem(LocalizationManager.GetResource("Email"), 0xE715.ToSegoeMDL2Icon(), () => share(SourceCodeShareType.Email));
            sub.Items?.AddItem(LocalizationManager.GetResource("SaveFile"), 0xED43.ToSegoeMDL2Icon(), () => share(SourceCodeShareType.LocalFile));
            menu.Items?.Add(sub);
            menu.AddSeparator();
            menu.Items?.AddItem(LocalizationManager.GetResource("Delete"), 0xE107.ToSegoeMDL2Icon(), delete);
            return menu;
        }
    }
}
