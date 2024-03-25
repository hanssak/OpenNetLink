using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.Models.SGNetwork
{
    public interface ISGNetwork
    {
        public int GroupID { get; set; }               
        public string FromName { get; set; }    
        public string ToName { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public int ConnectType { get; set; }
        //public string TlsVersion { get; set; }
        //public string ClientVersion { get; set; }
        public string APIVersion { get; set; }                  //RestAPI 서비스 정보
        public List<string> APIAddress { get; set; }            //RestAPI IP 주소
        public string NetPos { get; set; }
        public List<string> ADDomain { get; set; }
    }
}
