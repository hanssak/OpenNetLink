using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace OpenNetLinkApp.Models.SGSideBar
{
    public enum LSIDEBAR : int
    {
       [Description("HOME")]            MENU_CATE_ROOT      = 0,
       [Description("자료전송")]          MENU_CATE_FILE,
       [Description("메일")]             MENU_CATE_MAIL,
       [Description("PCURL")]           MENU_CATE_PCURL,
       [Description("클립보드")]          MENU_CATE_CLIP,
       [Description("환경설정")]          MENU_CATE_ENVSET,
       [Description("마지막카테고리")]     MENU_CATE_MAX,
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