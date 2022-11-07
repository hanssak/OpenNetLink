using OpenNetLinkApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    public class SGFileExceptionData
    {
        public bool bCheck { get; set; } = false;        // 체크 설정유무
        public string FileSeq { get; set; } = "";     // FileSeq

        public string systemID { get; set; } = "";       // 전송구분

        public string ApproveStatus { get; set; } = "";      // 승이상태

        public string FileName { get; set; } = "";      // FileName

        public string strDesc { get; set; } = "";      // 요청사유

        public string FileSize { get; set; } = "";        // File Size

        public string strInterLockFlagType { get; set; } = "";        // InterLockFlagType

        public string ReqTime { get; set; } = "";       // Req Time

        public string ApprDoneTime { get; set; } = "";       // Approve Done Time

        public string ExpiredDate { get; set; } = "";       // Expire Date


        /*public string getSizeStr()
        {
            string strRet = "";
            strRet = CsFunction.GetSizeStr(FileSize);
            return strRet;
        }*/

        public void initData()
        {
            bCheck = false;
        }

        public SGFileExceptionData() { }

    }
}
