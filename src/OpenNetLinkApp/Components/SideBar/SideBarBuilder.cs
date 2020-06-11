using Microsoft.AspNetCore.Components.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenNetLinkApp.Components.SideBar
{
    public class SideBarBuilder
    {
        private List<MenuItem> _menuItems;

        public SideBarBuilder()
        {
            _menuItems = new List<MenuItem>();
        }

        public SideBarBuilder AddItem(int position, string title, string link, string icon = null, string badge = null, NavLinkMatch match = NavLinkMatch.Prefix, bool isVisible = true, bool isEnabled = true)
        {
            var menuItem = new MenuItem
            {
                Position = position,
                Title = title,
                Link = link,
                Icon = icon,
                Badge = badge,
                BadgeValue = "0",
                Match = match,
                IsSubMenu = false,
                IsVisible = isVisible,
                IsEnabled = isEnabled
            };

            _menuItems.Add(menuItem);

            return this;
        }

        public SideBarBuilder AddSubMenu(int position, string title, SideBarBuilder menuItems, string icon = null, string badge = null, bool isVisible = true, bool isEnabled = true)
        {
            var menuItem = new MenuItem();
            menuItem.Position = position;
            menuItem.IsSubMenu = true;
            menuItem.Title = title;
            menuItem.Icon = icon;
            menuItem.Badge = badge;
            menuItem.BadgeValue = "0";
            menuItem.MenuItems = menuItems;
            menuItem.IsVisible = isVisible;
            menuItem.IsEnabled = isEnabled;

            _menuItems.Add(menuItem);
            return this;
        }

        internal List<MenuItem> Build(Func<MenuItem, int> orderBy)
        {
            var menuItems = _menuItems.OrderBy(orderBy);

            return menuItems.ToList();
        }
    }

    public class MenuItem
    {
        public int Position { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Icon { get; set; }
        public string Badge { get; set; }
        public string BadgeValue { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public NavLinkMatch Match { get; set; }
        public SideBarBuilder MenuItems { get; set; }
        public bool IsSubMenu { get; set; }
        public bool IsVisible { get; set; }
        public bool IsEnabled { get; set; }
    }
}
