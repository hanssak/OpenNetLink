using Microsoft.AspNetCore.Components;

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

        protected string CssString
        {
            get
            {
                var cssString = string.Empty;

                cssString += $"{Css}";
                cssString += !IsEnabled ? " disabled" : "";
                cssString += !IsVisible ? " hidden" : "";

                return cssString.Trim();
            }
        }
    }
}
