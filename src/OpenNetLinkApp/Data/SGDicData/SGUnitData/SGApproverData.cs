using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    public class ApproverCheckInfo
    {
        public string strUserSeq = "";
        public string strDeptSeq = "";

        public ApproverCheckInfo(string strUserSeqData, string strDeptSeqData)
        {
            strUserSeq = strUserSeqData;
            strDeptSeq = strDeptSeqData;
        }
    }

    public class ApproverApproveDataCheckInfo
    {
        public string strUserName = "";
        public string strWaitingTotalCount = "";
        public string strBlockPeriodOverCount = "";
        public string strWarningPeriodOverCount = "";
        public string strBlockPeriod = "";
        public string strWarningPeriod = "";

        public ApproverApproveDataCheckInfo(string strUserNameData, string strWaitingTotalCountData, string strBlockPeriodOverCountData, string strWarningPeriodOverCountData, string strBlockPeriodData, string strWarningPeriodData)
        {
            strUserName = strUserNameData;
            strWaitingTotalCount = strWaitingTotalCountData;
            strBlockPeriodOverCount = strBlockPeriodOverCountData;
            strWarningPeriodOverCount = strWarningPeriodOverCountData;
            strBlockPeriod = strBlockPeriodData;
            strWarningPeriod = strWarningPeriodData;
        }
    }




}
