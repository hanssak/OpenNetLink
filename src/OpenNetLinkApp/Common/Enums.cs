using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetLinkApp.Common
{
    class Enums
    {
        public enum EnumNetWorkType
        {
            [Description("None")]
            None = 0,
            [Description("단일망")]
            Single = 1,
            [Description("다중망")]
            Multiple = 2
        }
        public enum EnumBasicPageType : Int32
        {
            [Description("Main")]
            Main = 0,
            [Description("SideBar")]
            SideBar = 1
        }

    }
}
