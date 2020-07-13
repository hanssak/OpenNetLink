using System;

namespace OpenNetLinkApp.Models.SGUserInfo
{
    public interface ISGUserInfoAdded
    {
        
    }
    internal class SGUserInfoAdded : ISGUserInfoAdded
    {
        public SD_POLICY FileSendPolicy { get; private set; } = SD_POLICY.ONLY_OUT;  /* 사용자의 파일 전송에 대한 권한 설정. 1:반입만 2:반출만 3:전체허용 4:전체금지 5:부서상속 */
        public string FileFilterExt { get; private set; } = String.Empty; /* 확장자 제한  ex) exe;dll;com */
        public  long  FileSizeLimit { get; private set; } = 1536;       /* 파일저송 사이즈 제한 단위: MB, Default: 1.5GB */
        public  int   FileCountLimit { get; private set; } = 1024;    /* 전송 파일 갯수 제한 */
        public  SD_POLICY ClipSendPolicy { get; private set; } = SD_POLICY.ONLY_OUT;   /* 1: 외부->내부 2: 내부->외부 3: 전체허용 4: 전체금지 5: 부서상속 */
        public long  DayFileSizeLimit { get; private set; } = 1536; /* 하루에 전송 가능한 파일 최대 크기 */
        public int DayFileCountLimit { get; private set; } = 1024;  /* 하루에 전송 가능한 파일 최대 회수 */
        public long ClipSizeLimit { get; private set; } = 1536;
        public long DayClipSizeLimit { get; private set; } = 1536;
        public int DayClipCountLimit { get; private set; } = 1024;
        public long BandWidth { get; private set; } = 0;
        public bool IsUseApprove { get; private set; } = false;
        public bool IsUseAfterApprove { get; private set; } = false;
        public int MaxDownloadCount { get; private set; } = 1;
        public bool IsUsePcUrl { get; private set; } = false;
        public string GpkiCn { get; private set; } = String.Empty;
        public SD_POLICY  UrlSendPolicy { get; private set; } = SD_POLICY.ONLY_OUT; /* URL리다이렉션 사용 유무 */        
    }
}