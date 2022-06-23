using System;

namespace OpenNetLinkApp.Models.SGUserInfo
{
    public interface ISGUserInfoAdded
    {
        //SD_POLICY FileSendPolicy { get; }             /* 사용자의 파일 전송에 대한 권한 설정. 1:반입만 2:반출만 3:전체허용 4:전체금지 5:부서상속 */
        string FileFilterExt { get; }                   /* 확장자 제한  ex) exe;dll;com */
        long  FileSizeLimit { get; }                    /* 파일저송 사이즈 제한 단위: MB, Default: 1.5GB */
        int   FileCountLimit { get;}                    /* 전송 파일 갯수 제한 */
        //SD_POLICY ClipSendPolicy { get; }             /* 1: 외부->내부 2: 내부->외부 3: 전체허용 4: 전체금지 5: 부서상속 */
        long  DayFileSizeLimit { get;}                  /* 하루에 전송 가능한 파일 최대 크기 */
        int DayFileCountLimit { get; }                  /* 하루에 전송 가능한 파일 최대 회수 */
        long ClipSizeLimit { get; }
        long DayClipSizeLimit { get; }
        int DayClipCountLimit { get; }
        //long BandWidth { get; }
        //bool IsUseApprove { get; }
        //bool IsUseAfterApprove { get; }
        int MaxDownloadCount { get; }
        //bool IsUsePcUrl { get; }
        //string GpkiCn { get; }
        //SD_POLICY  UrlSendPolicy { get; }             /* URL리다이렉션 사용 유무 */
        bool IsMySelfSFM { get; }
    }
    internal class SGUserInfoAdded : ISGUserInfoAdded
    {
        //public SD_POLICY FileSendPolicy { get; private set; } = SD_POLICY.ONLY_OUT;  /* 사용자의 파일 전송에 대한 권한 설정. 1:반입만 2:반출만 3:전체허용 4:전체금지 5:부서상속 */
        public string FileFilterExt { get; set; } = String.Empty; /* 확장자 제한  ex) exe;dll;com */
        public  long  FileSizeLimit { get; set; } = 1536;       /* 파일저송 사이즈 제한 단위: MB, Default: 1.5GB */
        public  int   FileCountLimit { get; set; } = 1024;    /* 전송 파일 갯수 제한 */
        //public  SD_POLICY ClipSendPolicy { get; private set; } = SD_POLICY.ONLY_OUT;   /* 1: 외부->내부 2: 내부->외부 3: 전체허용 4: 전체금지 5: 부서상속 */
        public long  DayFileSizeLimit { get; set; } = 1536; /* 하루에 전송 가능한 파일 최대 크기 */
        public int DayFileCountLimit { get; set; } = 1024;  /* 하루에 전송 가능한 파일 최대 회수 */
        public long ClipSizeLimit { get; set; } = 1536;
        public long DayClipSizeLimit { get; set; } = 1536;
        public int DayClipCountLimit { get; set; } = 1024;
        //public long BandWidth { get; private set; } = 0;
        //public bool IsUseApprove { get; private set; } = false;
        //public bool IsUseAfterApprove { get; private set; } = false;
        public int MaxDownloadCount { get; set; } = 1;
        //public bool IsUsePcUrl { get; private set; } = false;
        //public string GpkiCn { get; private set; } = String.Empty;
        //public SD_POLICY  UrlSendPolicy { get; private set; } = SD_POLICY.ONLY_OUT; /* URL리다이렉션 사용 유무 */

        public bool IsMySelfSFM { get; set; } = false; // 자신이 대결재자로 지정되어있는지 여부
    }
}
