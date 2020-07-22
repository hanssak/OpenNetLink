using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.Models.SGNetwork
{
    public interface ISGNetwork
    {
        public int GroupID { get; set; }               
        public string Name { get; set; }                 
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public int ConnectType { get; set; }
    }
}
