using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;

namespace OpenNetLinkApp.Components.SideBar
{
    public class SideBarMenuItemBase : ComponentBase
    {
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public bool IsEnabled { get; set; } = true;
        [Parameter] public bool IsVisible { get; set; } = true;
        [Parameter] public string Css { get; set; } = string.Empty;
        [Parameter] public string Link { get; set; } = string.Empty;
        [Parameter] public string Title { get; set; } = string.Empty;
        [Parameter] public string Icon { get; set; } = string.Empty;
        [Parameter] public string Badge { get; set; } = string.Empty;
        [Parameter] public string BadgeValue { get; set; } = string.Empty;
        [Parameter] public MenuItem MenuItem { get; set; }
        [Parameter] public SideBarBuilder MenuBuilder { get; set; }
        protected string CssActive { get; set; } = string.Empty;

        protected string CssString
        {
            get
            {
                var cssString = string.Empty;
                MenuItem.CssActive = string.Empty;

                cssString += $"{Css}";
                cssString += !IsEnabled ? " disabled" : "";
                cssString += !IsVisible ? " hidden" : "";

                MenuItem.CssActive += MenuItem.IsActive ? " active" : "";
                CssActive = MenuItem.CssActive;
                return cssString.Trim();
            }
        }

        public void OffTheActiveItem()
        {
            foreach (MenuItem item in MenuBuilder.Build(x => x.Position))
            {
                Console.WriteLine("> " + item.Title + "|" + MenuItem.Title + "|IsActM: " + MenuItem.IsActive);
                Console.WriteLine("---> item CssActive: " + item.CssActive + "|" + "MenuItem CssActive: " + MenuItem.CssActive );
                if(item.IsSubMenu == false && item.IsActive == true)
                {
                    //Console.WriteLine("> " + item.Title + "|" + MenuItem.Title + "|IsAct: " + item.IsActive + "|IsActM: " + MenuItem.IsActive);
                    item.IsActive = false;
                    item.CssActive = String.Empty;
                }
                else if (item.IsSubMenu == true)
                {
                    //Console.WriteLine("> " + item.Title + "+");
                    foreach (MenuItem itemSub in item.MenuItems.Build(x => x.Position))
                    {
                        //Console.WriteLine(">> " + itemSub.Title);
                        if(itemSub.IsSubMenu == false && itemSub.IsActive == true)
                        {
                            itemSub.IsActive = false;
                            itemSub.CssActive = String.Empty;
                        }
                    }
                }
            }
        }

        protected void ToggleMenuItem()
        {
            bool keepActive = MenuItem.IsActive;
            OffTheActiveItem();
            MenuItem.IsActive = keepActive;
            MenuItem.IsActive = !MenuItem.IsActive;
            MenuItem.CssActive = string.Empty;
            MenuItem.CssActive += MenuItem.IsActive ? " active" : "";
            StateHasChanged();
        }

        /// <summary>
        /// Handler for the key down events
        /// </summary>
        /// <param name="eventArgs">keyboard event</param>
        protected void KeyDownHandler(KeyboardEventArgs eventArgs)
        {
            if (eventArgs.Key == "Enter" || eventArgs.Key == " " || eventArgs.Key == "Spacebar")
            {
                ToggleMenuItem();
            }
        }
    }
}
