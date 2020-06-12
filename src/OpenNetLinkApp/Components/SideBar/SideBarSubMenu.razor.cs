using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;

namespace OpenNetLinkApp.Components.SideBar
{
    public class SideBarSubMenuBase : ComponentBase
    {
        [Parameter] public string Css { get; set; }
        [Parameter] public string Header { get; set; }
        [Parameter] public string Icon { get; set; }
        [Parameter] public string Badge { get; set; }
        [Parameter] public string BadgeValue { get; set; }
        [Parameter] public MenuItem MenuItem { get; set; }
        [Parameter] public bool IsEnabled { get; set; } = true;
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public RenderFragment HeaderTemplate { get; set; }
        [Parameter] public RenderFragment MenuTemplate { get; set; } 
        [Parameter] public IEnumerable<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        [Parameter] public SideBarBuilder MenuBuilder { get; set; }

        protected string LastIcon { get; set; } = "+";
        protected string CssActive { get; set; } = string.Empty;
        protected string CssString
        {
            get
            {
                var cssString = "";
                CssActive = "";

                cssString += $" {Css}";
                cssString += MenuItem.IsOpen ? " menu-open" : "";
                CssActive += MenuItem.IsOpen ? " active" : "";

                return cssString.Trim();
            }
        }

        public void OffTheActiveSub()
        {
            foreach (MenuItem item in MenuItems)
            {
                if(item.IsSubMenu == true && item.IsOpen == true) item.IsOpen = false;
            }
        }

        protected void ToggleSubMenu()
        {
            bool keepOpen = MenuItem.IsOpen;
            OffTheActiveSub();
            MenuItem.IsOpen = keepOpen;
            MenuItem.IsOpen = !MenuItem.IsOpen;
            LastIcon = MenuItem.IsOpen ? "-" : "+";
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
                ToggleSubMenu();
            }
        }
    }
}
