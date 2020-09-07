using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNetLinkApp.Data.SGQuery
{
    class SGQueryExtend
    {
        public SGQueryExtend()
        {

        }
        ~ SGQueryExtend()
        {

        }

        public string GetDeptApprLineSearch(string strSysID, string strUserName, string strTeamName, string strTeamCode, bool bApprPos)
        {
            string strApprPos = "0";
            if (bApprPos)
                strApprPos = "1";

            string strQuery = "";
            strQuery = String.Format("SELECT * FROM FUNC_USERINFO_SEARCH('{0}', '{1}', '{2}', '{3}', '{4}')", strUserName, strTeamName, strTeamCode,strApprPos,strSysID);
            return strQuery;
        }

        public string GetAgentBlock(string strUserSeq, string strSystemType, string strBlockType, string strBlockReason)
        {
            string strQuery = "";
            strQuery = String.Format("insert into tbl_agent_block values({0},'{1}','{2}','{3}',Now())\n", strUserSeq,strSystemType, strBlockType, strBlockReason);
            return strQuery;
        }
    }
}
