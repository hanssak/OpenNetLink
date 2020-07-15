using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.Models.SGNetwork
{
    internal class SGNetwork : ISGNetwork
    {
        public string IPAddress { get; set; }               // IP 주소
        public int nPort { get; set; }                       // Port 정보
        public int nConType { get; set; }                 // connect type 

    }

    internal class SGNetworkInfo : ISGNetworkInfo
    {
        public List<SGNetwork> SGNetworks { get; set; }
    }
}
