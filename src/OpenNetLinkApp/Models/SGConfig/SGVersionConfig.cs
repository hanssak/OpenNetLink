using Radzen.Blazor.Rendering;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using Serilog;
using Serilog.Events;
using AgLogManager;

namespace OpenNetLinkApp.Models.SGConfig
{
    public class SGVersionConfig : ISGVersionConfig
    {       
        public string LastUpdated { get; set; } = DateTime.Now.ToString(@"yyyy\/MM\/dd h\:mm tt"); // 마지막으로 업데이트된 날짜/시간정보
        public string SWVersion { get; set; } = "1.0.0.1";                                  // 소프트웨어 버전 정보
        public string SWCommitId { get; set; } = "ad9f269";                                 // 소프트웨어 버전 정보 : Git Commit Point for this Released S/W       
        public string UpdatePlatform { get; set; } = string.Empty;                          // 업데이트 될 OpenNetLinkApp Machine Architecture 플랫폼   
    }
}
