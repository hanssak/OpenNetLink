using OpenNetLinkApp.Models.SGNotify;

namespace OpenNetLinkApp.Models.SGHeader
{
    internal class SGHeaderUI : ISGHeaderUI
    {
        public string HomePath { get; private set; }
        public SGNotiInfoUI NotiInfo  { get; private set; }
        public SGAlarmInfoUI AlarmInfo  { get; private set; }        
    }
}