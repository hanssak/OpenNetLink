namespace OpenNetLinkApp.Models.SGConfig
{
    public interface ISGVersionConfig
    {        
        string LastUpdated { get; }                                 // 마지막으로 업데이트된 날짜/시간정보
        string SWVersion { get; set; }                                   // 소프트웨어 버전 정보
        string SWCommitId { get; }                                  // 소프트웨어 버전 정보 : Git Commit Point for this Released S/W       
        string UpdatePlatform { get; set; }                         // 업데이트 될 OpenNetLinkApp Machine Architecture 플랫폼
    }
}
