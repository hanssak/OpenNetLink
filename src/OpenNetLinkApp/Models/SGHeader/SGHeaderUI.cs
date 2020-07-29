using System;

namespace OpenNetLinkApp.Models.SGHeader
{
    internal class SGHeaderUI : ISGHeaderUI
    {
        public SGNotiInfoUI NotiInfo  { get; private set; } = new SGNotiInfoUI();
        public SGAlarmInfoUI AlarmInfo  { get; private set; } = new SGAlarmInfoUI();
    }
}