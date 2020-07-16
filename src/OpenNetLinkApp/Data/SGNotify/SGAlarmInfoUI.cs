using System;
using System.Collections.Concurrent;
using OpenNetLinkApp.Models.SGNotify;

namespace OpenNetLinkApp.Data.SGNotify
{
    public class SGAlarmInfoUI
    {
        public string AlarmPath { get; private set; } = String.Empty;
        public string Icon { get; private set; } = String.Empty;
        public string Badge { get; private set; } = String.Empty;
        public ConcurrentQueue<SGAlarmData> AlarmQ  { get; private set; } = new ConcurrentQueue<SGAlarmData>();
    }
}