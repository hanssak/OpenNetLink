using System;
using OpenNetLinkApp.Data.SGNotify;

namespace OpenNetLinkApp.Models.SGHeader
{
    public interface ISGHeaderUI
    {
        string HomePath { get; }
        SGNotiInfoUI NotiInfo  { get; }
        SGAlarmInfoUI AlarmInfo  { get; }        
    }
}