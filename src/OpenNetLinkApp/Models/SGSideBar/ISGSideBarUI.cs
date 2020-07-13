using System;
using System.Collections.Generic;

namespace OpenNetLinkApp.Models.SGSideBar
{
    public interface ISGSideBarUI
    {
        int             GroupId         { get; }
        int             MenuId          { get; }
        ISGSideBarUI    Parent          { get; }
        string 		    Name            { get; }
        string 		    Icon            { get; }
        string 		    Path            { get; }
        string 		    ToolTip         { get; }
        string 		    MenuOpenClass   { get; }
        string 		    ActiveClass     { get; }
        string     		BadgeType       { get; }
        string     		BadgeValue      { get; }
        bool 		    Actived         { get; }
        bool 		    Expanded        { get; }
        bool 		    IsSubMenu       { get; }
        
        List<ISGSideBarUI> Child        { get; }     
    }
}