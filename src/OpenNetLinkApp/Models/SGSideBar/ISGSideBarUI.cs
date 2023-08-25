using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace OpenNetLinkApp.Models.SGSideBar
{
    public enum LSIDEBAR : int
    {
       [Description("T_LSIDEBAR_MENU_HOME_NAME")]           MENU_CATE_ROOT      = 0,
       [Description("T_COMMON_FILETRANS_BASIC")]            MENU_CATE_FILE,
       [Description("T_COMMON_EMAIL")]                      MENU_CATE_MAIL,
       [Description("T_COMMON_URL")]                        MENU_CATE_PCURL,
       [Description("T_TOOL_CLIPBOARD")]                    MENU_CATE_CLIP,
       [Description("T_COMMON_OPTION")]                     MENU_CATE_ENVSET,
       [Description("T_LSIDEBAR_MENU_LAST_NAME")]           MENU_CATE_MAX,
    }

    public interface ISGSideBarUI
    {
        int             GroupId         { get; }
        int             Idx             { get; }
        int             ParentId        { get; }
        LSIDEBAR        CategoryId      { get; }
        ISGSideBarUI    Parent          { get; }
        string 		    FromName        { get; }
        string 		    ToName          { get; }
        string 		    Icon            { get; }
        string 		    Path            { get; set; }
        string 		    ToolTip         { get; }
        string 		    MenuOpenClass   { get; }
        string 		    ActiveClass     { get; }
        string     		BadgeType       { get; }
        string     		BadgeValue      { get; }
        bool 		    Actived         { get; }
        bool 		    Expanded        { get; set; }
        bool 		    IsSubMenu       { get; }
        string          strItemUserSeq      { get; }
        List<ISGSideBarUI>   Child      { get; }     
    }
}