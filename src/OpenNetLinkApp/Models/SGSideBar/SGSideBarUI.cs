using System;
using System.Collections.Generic;

namespace OpenNetLinkApp.Models.SGSideBar
{
    internal class SGSideBarUI : ISGSideBarUI
    {
        public int              GroupId         { get; set; }
        public int              Idx             { get; set; }
        public int              ParentId        { get; set; }
        public LSIDEBAR         CategoryId      { get; set; }
        public ISGSideBarUI 	Parent          { get; set; }
        public string 		    FromName        { get; set; }
        public string 		    ToName          { get; set; }
        public string 		    Icon            { get; set; }
        public string 	    	Path            { get; set; }
        public string 		    ToolTip         { get; set; }
        public string 		    MenuOpenClass   { get { return Expanded?"menu-open":"";} }
        public string 		    ActiveClass     { get { return Actived?"active":"";} }
        public string     		BadgeType       { get; set; }
        public string     		BadgeValue      { get; set; }
        public bool 		    Actived         { get; set; }
        public bool 		    Expanded        { get; set; }
        public bool 		    IsSubMenu       { get; set; }
        
        public List<ISGSideBarUI>   Child       { get; set; }     
    }
}