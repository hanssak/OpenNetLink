using OpenNetLinkApp.Common;
using OpenNetLinkApp.Data.SGDicData.SGUnitData;
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
            string sql = $@"Hsck_Transaction:DELETE FROM TBL_USER_SFM WHERE USER_SEQ = {userSeq}
";
            foreach (ApproverInfo approver in approverInfos)
            {
                //sb.Append($@"INSERT INTO TBL_USER_SFM VALUES ({userSeq} , {approver.UserSeq}, '{stStartTime.Replace("-", "")}', '{stEndTime.Replace("-", "")}',  CAST (to_char(now(), 'YYYYMMDDHH24MISS') AS VARCHAR), {userSeq} )\n");
                sql += $@"INSERT INTO TBL_USER_SFM VALUES ({userSeq} , {approver.UserSeq}, '{stStartTime.Replace("-", "")}', '{stEndTime.Replace("-", "")}',  CAST (to_char(now(), 'YYYYMMDDHH24MISS') AS VARCHAR), {userSeq} )
";                 
            }
            return CsFunction.GetChangeNewLineToN(sql);
        }

        public string GetSFMApprover(string userSeq)
        {
            string sql = $@"SELECT * FROM FUNC_SFM_APPROVE_SEARCH({userSeq})";

            return sql;
        }

        public string GetSFMDeptCount(string userSeq)
        {
            string sql = $@"
SELECT (CASE WHEN D.LIMIT_SFM_NUM IS NULL THEN '3' ELSE D.LIMIT_SFM_NUM END) LIMITCNT
					 FROM TBL_DEPT_INFO D, TBL_USER_INFO U
					 WHERE D.DEPT_SEQ=U.DEPT_SEQ
					 AND U.USER_SEQ = {userSeq}
";
            return sql;
        }

        public string GetSFMApporverRight(string userSeq)
        {
            string sql = $"SELECT * FROM FUNC_SFM_USER_APPROVE_RIGHT({userSeq})";

            return sql;
        }
    }
}
