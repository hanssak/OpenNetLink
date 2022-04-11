using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.Models.SGNetwork
{
    public class SGNetwork : ISGNetwork
    {
        public int GroupID { get; set; }                    // 망 그룹 ID
        public string FromName { get; set; }                // 출발지 망 이름 
        public string ToName { get; set; }                  // 목적지 망 이름
        public string IPAddress { get; set; }               // IP 주소
        public int Port { get; set; }                       // Port 정보
        public int ConnectType { get; set; }                // connect type 
        public string TlsVersion { get; set; }              // TLSVersion 정보
        public string ClientVersion { get; set; }           // ClientVersion 정보
    }

}
