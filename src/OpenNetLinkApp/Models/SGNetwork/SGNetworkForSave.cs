using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetLinkApp.Models.SGNetwork
{
    /// <summary>
    /// Network.json을 관리하는 객체는 "NETWORKS" 노트는 따로 관리되지 않아, 
    /// 파일 저장을 위해 Class 객체 생성하여 사용
    /// </summary>
    public class SGNetworkForSave
    {
        public List<ISGNetwork> NETWORKS { get; set; }
    }
}
