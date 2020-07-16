using System;
using OpenNetLinkApp.Data.SGNotify;

namespace OpenNetLinkApp.Models.SGHeader
{
    internal class SGHeaderUI : ISGHeaderUI
    {
        public string HomePath { get; private set; } = String.Empty;
        public SGNotiInfoUI NotiInfo  { get; private set; } = new SGNotiInfoUI();
        public SGAlarmInfoUI AlarmInfo  { get; private set; } = new SGAlarmInfoUI();
    }
}