using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.Models.SGNetwork
{
    public interface ISGNetwork
    {
        public string IPAddress { get; }
        public int Port { get; }
        public string ConType { get; }
    }
    public interface ISGNetworkInfo
    {
        public List<ISGNetwork> SGNetworks { get { return SGNetworks; } }
    }
}
