using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.Models.SGNetwork
{
    public interface ISGNetwork
    {
        public string IPAddress { get; }
        public int nPort { get; }
        public int nConType { get; }
    }
    public interface ISGNetworkInfo
    {
        public List<ISGNetwork> SGNetworks { get { return SGNetworks; } }
    }
}
