using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetLinkApp.Common
{
    public class Enums
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

        public enum EnumApproveTime : Int32
        {
            [Description("전체")]
            All = 0,
            [Description("사전")]
            Before = 1,
            [Description("사후")]
            After = 2
        }

        public enum EnumPageView : Int32
        {
            [Description("전체")]
            All = 0,
            [Description("일반결재")]
            ApproveUI = 1,
            [Description("보안결재")]
            SecurityApproveUI = 2,
            [Description("클립보드결재")]
            ClipApproveUI = 3,
            [Description("파일전송 예외처리")]
            FileException = 4,
        }
    }
}
