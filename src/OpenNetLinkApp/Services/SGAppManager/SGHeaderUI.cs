namespace OpenNetLinkApp.Services.SGAppManager
{
    public interface ISGHeaderUI
    {

    }
    internal class SGHeaderUI : ISGHeaderUI
    {
        public string HomePath { get; private set; }
        public SGNotiInfoUI NotiInfo  { get; private set; }
        public SGAlarmInfoUI AlarmInfo  { get; private set; }        
    }
}