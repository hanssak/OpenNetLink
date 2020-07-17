using System;
using System.Collections.Concurrent;

namespace OpenNetLinkApp.Models.SGNotify
{
    public class SGAlarmData
    {
        public string Path { get; private set; } = String.Empty;
        public string IconImage { get; private set; } = String.Empty;
        public string Head { get; private set; } = String.Empty;
        public string Body { get; private set; } = String.Empty;
        public DateTime? Time { get; private set; } = null; 
    }
}