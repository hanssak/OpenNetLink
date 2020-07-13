using System.Collections.Concurrent;

namespace OpenNetLinkApp.Models.SGNotify
{
    public class SGNotiQData
    {
        public string Title{ get; private set; }
        public string Body{ get; private set; }
        public string ImagePath{ get; private set; }
    }
    public class SGNotiInfoUI
    {
        public ConcurrentQueue<SGNotiQData> NotiQ  { get; private set; } = new ConcurrentQueue<SGNotiQData>();
    }
}