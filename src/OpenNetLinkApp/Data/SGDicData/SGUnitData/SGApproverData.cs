using HsNetWorkSGData;
using Serilog;
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

        /// <summary>
        /// 사용자id ( 동명이인 때문에 꼭 필요할때에만 사용 )
        /// </summary>
        public string strUserid = "";

        public string strUserName = "";

        /// <summary>
        /// 
        /// </summary>
        public string strUserRank = "";

        /// <summary>
        /// 이 사용자의 사후결재 전체 대기건수
        /// </summary>
        public string strWaitingTotalCount = "";

        /// <summary>
        /// 사후 결재 제한 대기기간 오버한 갯수
        /// </summary>
        public string strBlockPeriodOverCount = "";

        /// <summary>
        /// 사후 결재 경고 대기기간 포함된 갯수
        /// </summary>
        public string strWarningPeriodOverCount = "";

        /// <summary>
        /// 설정된 사후 결재 제한 대기기간
        /// </summary>
        public string strBlockPeriod = "";

        /// <summary>
        /// 설정된 사후 결재 경고 대기기간
        /// </summary>
        public string strWarningPeriod = "";

        public ApproverApproveDataCheckInfo()
        {

        }

        public ApproverApproveDataCheckInfo(string strUseridData, string strUserNameData, string strUserRankData, string strWaitingTotalCountData, string strBlockPeriodOverCountData, string strWarningPeriodOverCountData, string strBlockPeriodData, string strWarningPeriodData)
        {
            strUserid = strUseridData;
            strUserName = strUserNameData;
            strUserRank = strUserRankData;
            strWaitingTotalCount = strWaitingTotalCountData;
            strBlockPeriodOverCount = strBlockPeriodOverCountData;
            strWarningPeriodOverCount = strWarningPeriodOverCountData;
            strBlockPeriod = strBlockPeriodData;
            strWarningPeriod = strWarningPeriodData;
        }

    }




}
