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
        [Parameter] public bool IsEnabled { get; set; } = true;
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public RenderFragment HeaderTemplate { get; set; }
        [Parameter] public RenderFragment MenuTemplate { get; set; } 
        [Parameter] public IEnumerable<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

        protected string LastIcon { get; set; } = "+";
        protected bool IsOpen { get; set; }
        protected string CssActive { get; set; } = string.Empty;
        protected string CssString
        {
            get
            {
                var cssString = "";
                CssActive = "";

                cssString += $" {Css}";
                cssString += IsOpen ? " menu-open" : "";
                CssActive += IsOpen ? " active" : "";

                return cssString.Trim();
            }
        }

        protected void ToggleSubMenu()
        {
            IsOpen = !IsOpen;
            LastIcon = IsOpen ? "-" : "+";
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
