using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetLinkApp.Data.SGDicData.Approve
{
    public class ApprovalInfo
    {
        public ApprovalInfo(string userSeq, string userID, string userName, string approveStatusCode, string approveStatusName, string approveReason, string approveTime)
        {
            UserSeq = userSeq;
            UserID = userID;
            UserName = userName;
            ApproveStatusCode = approveStatusCode;
            ApproveStatusName = approveStatusName;
            ApproveReason = approveReason;
            ApproveTime = approveTime;
        }

        public string UserSeq { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string ApproveStatusCode { get; set; }
        public string ApproveStatusName { get; set; }
        public string ApproveReason { get; set; }
        public string ApproveTime { get; set; }
    }
}
