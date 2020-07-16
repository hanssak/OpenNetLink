using System;
using System.Collections.Concurrent;
using OpenNetLinkApp.Models.SGNotify;

namespace OpenNetLinkApp.Data.SGNotify
{
    public class SGNotiInfoUI
    {
        public string NotiPath { get; private set; } = String.Empty;
        public string Icon { get; private set; } = String.Empty;
        public string Badge { get; private set; } = String.Empty;
        public ConcurrentQueue<SGNotiData> NotiQ  { get; private set; } = new ConcurrentQueue<SGNotiData>();
    }
}