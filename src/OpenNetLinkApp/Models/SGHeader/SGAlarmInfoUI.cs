using System;
using System.Collections;

namespace OpenNetLinkApp.Models.SGHeader
{
    public class SGAlarmInfoUI
    {
        public string AlarmPath { get; private set; } = "";
        public string Icon { get; private set; } = "fas fa-bell";
        public string Badge { get; private set; } = "badge badge-danger";
    }
}
