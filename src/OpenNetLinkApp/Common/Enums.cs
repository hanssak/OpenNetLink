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
        [Flags]
        public enum EnumSysPos
        {
            [Description("None")]
            None = 0,
            [Description("ProgramFiles 위치")]
            ProgramFiles = 1,
            [Description("UserData 위치")]
            UserData = 2,
            [Description("다른 Volume Drive 위치")]
            OtherDrive = 3,
            [Description("ProgramFilesX86 위치")]
            ProgramFilesX86 = 4
        }

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
            [Description("일반결재 사후")]
            ApproveUIAfter = 5,
            [Description("메일결재")]
            MailApproveUI = 6,
            [Description("메일결재 승인대기조회")]
            MailApproveWaitUI = 7,
            [Description("일반결재 승인대기조회")]
            ApproveWaitAllUI = 8,
            [Description("메일보안결재 승인대기조회")]
            MailSecurityApproveWaitUI = 9,
            [Description("메일보안결재")]
            MailSecurityApproveUI = 10
        }
        /// <summary>
        /// 승인/반려
        /// </summary>
        public enum EnumApproveType : Int32
        {
            [Description("승인")]
            Approve = 1,
            [Description("반려")]
            Reject = 2,

        }
        /// <summary>
        /// 검사단계표시 타입
        /// <para>포맷 : 전체검사단계/현재검사단계 (INTERLOCKFLAG 값 기준으로 표시) </para>
        /// </summary>
        [Flags]
        public enum PreworkType : Int32
        {
            [Description("초기값")]
            BLOCK_NONE = 0,
            [Description("APT 검사")]
            APT_SCAN = 1,
            [Description("바이러스 검사")]
            VIRUS_SCAN = 2,
            [Description("DRM 검사")]
            DRM_SCAN = 4,
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

        /// <summary>
        /// groupID 별 업데이트 상태체크
        /// </summary>
        public enum UpdateStatusType
        {
            /// <summary>
            /// INIT 또는 업데이트 중 취소
            /// </summary>
            NONE = 0,
            /// <summary>
            /// 업데이트 체크 중
            /// </summary>
            CHECKING = 1,
            /// <summary>
            /// 패치 버전 다운로드 중
            /// </summary>
            DOWNLOADING =2,
            /// <summary>
            /// 설치 중
            /// </summary>
            INSTALLING =4,
        }
    }
}
