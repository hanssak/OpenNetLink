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

        /// <summary>
        /// 검사단계표시 타입
        /// <para>포맷 : 전체검사단계/현재검사단계 (INTERLOCKFLAG 값 기준으로 표시) </para>
        /// </summary>
        public enum PreworkType : Int32
        {
            [Description("APT 검사")]
            APT_SCAN = 1,
            [Description("바이러스 검사")]
            VIRUS_SCAN = 2,
            [Description("DRM 검사")]
            DTM_SCAN = 4,
            [Description("개인정보 검사")]
            PERSONAL_DATA_SCAN = 8,
        }

        /// <summary>
        /// 문서파일 첨부파일 검사 타입
        /// <para>OLEOBJECT_EXTRACT : OLE 개체 검사 (문서 편집기 내)</para>
        /// </summary>
        [Flags]
        public enum DocumentExtractType
        {
            NONE = 0,
            /// <summary>
            /// OLE 개체 마임리스트 검사
            /// </summary>
            OLEOBJECT_EXTRACT = 1,

            /// <summary>
            /// OLE 개체 위변조 검사
            /// </summary>
            OLEOBJECT_EXTEXCHANGE_EXTRACT=2,
            //COMPRESS_EXTRACT = 2,
            
            
        }
    }
}
