using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.Models.SGNetwork
{
    public class SGNetwork : ISGNetwork
    {
        public int GroupID { get; set; }                    // 망 그룹 ID
        public string Name { get; set; }                    // 망 이름 
        public string IPAddress { get; set; }               // IP 주소
        public int Port { get; set; }                       // Port 정보
        public int ConnectType { get; set; }                 // connect type 

    }

}
