using System.Collections.Concurrent;

namespace OpenNetLinkApp.Services.SGAppManager
{
    public class SGAlarmQData
    {
        public string Title{ get; private set; }
        public string Body{ get; private set; }
        public string ImagePath{ get; private set; }
    }
    public class SGAlarmInfoUI
    {
        public ConcurrentQueue<SGAlarmQData> AlarmQ  { get; private set; } = new ConcurrentQueue<SGAlarmQData>();
    }

}