using HsNetWorkSG;
using OpenNetLinkApp.Common;
using OpenNetLinkApp.Data.SGDicData.SGUnitData;
using OpenNetLinkApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenNetLinkApp.Data.SGQuery
{
    public class ApproveProxy
    {
        public string GetSFMModify(string userSeq, string stStartTime, string stEndTime, List<ApproverInfo> approverInfos)
        {
            Dictionary<string, string> param = new Dictionary<string, string>() { { "userSeq", userSeq } };

            StringBuilder sb = new StringBuilder();
            SQLXmlService.Instanse.GetSqlQuery("SFMModifyDelete", param, ref sb);
            foreach (ApproverInfo approver in approverInfos)
            {
                Dictionary<string, string> subParam = new Dictionary<string, string>() { 
                    { "userSeq", userSeq }, { "ApprUserSeq", approver.UserSeq}, { "stStartTime", stStartTime.Replace("-", "") },
                    { "stEndTime", stEndTime.Replace("-", "")}
                };
                StringBuilder subSb = new StringBuilder();
                SQLXmlService.Instanse.GetSqlQuery("SFMModifyInsert", subParam, ref subSb);
                sb.Append(subSb.ToString());
                subSb.Clear();
            }
            sb.Replace(Environment.NewLine, "");
            sb.Replace("\t", "");
            sb.Replace("\\n", "\n");
            string sql = sb.ToString();
            return SGCrypto.AESEncrypt256WithDEK(ref sql);
        }

        public string GetSFMApprover(string userSeq)
        {
            Dictionary<string, string> param = new Dictionary<string, string>() { { "userSeq", userSeq } };

            return SQLXmlService.Instanse.GetSqlQuery("SFMApprover", param);
        }

        public string GetSFMDeptCount(string userSeq)
        {
            Dictionary<string, string> param = new Dictionary<string, string>() { { "userSeq", userSeq } };

            return SQLXmlService.Instanse.GetSqlQuery("SFMDeptCount", param);
        }

        public string GetSFMApproverRight(string userSeq)
        {

            Dictionary<string, string> param = new Dictionary<string, string>() { { "userSeq", userSeq } };

            return SQLXmlService.Instanse.GetSqlQuery("SFMApproverRight", param);
        }
    }
}
