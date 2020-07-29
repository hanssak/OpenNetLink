using System;
using OpenNetLinkApp.Data.SGNotify;

namespace OpenNetLinkApp.Models.SGHeader
{
    public interface ISGHeaderUI
    {
        SGNotiInfoUI NotiInfo  { get; }
        SGAlarmInfoUI AlarmInfo  { get; }        
    }
}
