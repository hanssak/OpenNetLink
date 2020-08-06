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

            
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  REPLACE(SPLIT_PART(TAG_VALUE, ',', 3), '/', ',')  INTO ORDERCOL ");
            sb.Append("FROM TBL_SYSTEM_ENV ");
            sb.Append("WHERE TAG='DEPTQUERYTYPE' ");
            if (!(strSysID.Equals("")))
            {
                sb.Append("AND SYSTEM_ID=" + strSysID +"|| '001' ");
            }

            sb.Append("WITH TBL_TMP_USER AS ");
            sb.Append("( SELECT * FROM TBL_USER_INFO A WHERE A.USE_STATUS = ''1'' AND COALESCE(A.ACCOUNT_EXPIRES, ''99991231'') > TO_CHAR(NOW(), ''YYYYMMDD'')) ");
            sb.Append("SELECT  A.USER_ID, A.USER_NAME, A.DEPT_SEQ, A.DEPT_NAME, ");
            //sb.Append("-- (CASE WHEN A.ORG_DEPT_NAME ='''' OR A.ORG_DEPT_NAME IS NULL THEN A.DEPT_NAME ELSE A.ORG_DEPT_NAME END) DEPT_NAME, ");
            sb.Append("(CASE WHEN A.USER_POSITION IS NULL THEN ''-'' ");
            sb.Append("WHEN A.USER_POSITION='''' THEN ''-'' ");
            sb.Append("WHEN A.USER_POSITION='' '' THEN ''-'' ");
            sb.Append("ELSE A.USER_POSITION END) USER_POSITION, ");
            sb.Append("(CASE WHEN A.USER_RANK IS NULL THEN ''-'' ");
            sb.Append("WHEN A.USER_RANK='''' THEN ''-'' ");
            sb.Append("WHEN A.USER_RANK='' '' THEN ''-'' ");
            sb.Append("ELSE A.USER_RANK END) USER_RANK, ");
            sb.Append("A.PART_OWNER, A.APPRPOS, A.USER_SEQ, A.DLP_APPROVE ");
            sb.Append("FROM ( ");
            sb.Append("SELECT A.USER_ID, 		A.USER_NAME, 	A.DEPT_SEQ, B.DEPT_NAME, 	'''' ORG_DEPT_NAME, 	A.USER_POSITION, ");
            sb.Append("A.USER_RANK, 	A.PART_OWNER, 	A.APPRPOS, 	A.USER_SEQ, 	A.DLP_APPROVE, 			A.USER_SORT ");
            sb.Append("FROM TBL_TMP_USER A ");
            sb.Append(", TBL_DEPT_INFO B ");
            sb.Append("WHERE A.DEPT_SEQ = B.DEPT_SEQ ");
            sb.Append("UNION ALL ");
            sb.Append("SELECT A.USER_ID, 		A.USER_NAME, 	D.DEPT_SEQ, D.DEPT_NAME, 	OD.DEPT_NAME ORG_DEPT_NAME, A.USER_POSITION, A.USER_RANK, 	A.PART_OWNER, ");
            sb.Append("( CASE WHEN A.APPRPOS = 0 OR A.APPRPOS IS NULL THEN 2 ELSE A.APPRPOS END) APPRPOS, A.USER_SEQ, A.DLP_APPROVE, A.USER_SORT ");
            sb.Append("FROM TBL_OTHER_APPROVE OA, TBL_TMP_USER A, TBL_DEPT_INFO , TBL_DEPT_INFO OD ");
            sb.Append("WHERE OA.USER_SEQ=A.USER_SEQ AND OA.O_ORDER > 0 AND OA.DEPT_SEQ = D.DEPT_SEQ AND A.DEPT_SEQ = OD.DEPT_SEQ ");
            sb.Append(") A ");
            sb.Append("WHERE ");

            if (!(strUserName.Equals("")))
            {
                sb.Append("  A.USER_NAME LIKE ''%' || '" + strUserName + "' || '%'");
            }

            if (!(strTeamName.Equals("")))
            {
                sb.Append("AND  A.DEPT_NAME LIKE ''%' || '" + strTeamName + "' || '%'");
            }

            if (!(strTeamCode.Equals("")))
            {
                sb.Append("AND  A.DEPT_SEQ LIKE ''%' || '" + strTeamCode + "' || '%'");
            }

            if (!(strApprPos.Equals("")))
            {
                sb.Append("AND  A.USER_NAME LIKE ''%' || '" + strApprPos + "' || '%'");
            }


            return sb.ToString();
            /*
            sb.Append("  func_transstatus(a.trans_flag, a.recv_flag, a.pctrans_flag) as transStatus, a.approve_kind as approveKind, a.approve_flag as approveFlag, a.title as title, a.file_size as fileSize, ");
            sb.Append("  func_transfilepos(a.system_id, a.trans_flag, a.recv_flag, a.pctrans_flag) as transPos, to_char(to_timestamp(substring(a.request_time, 1, 14),'YYYYMMDDHH24MISS'), 'YYYY-MM-DD HH24:MI:SS') as requestTime, ");
            sb.Append("  '1' as downPossible, To_char(To_timestamp(COALESCE(b.expired_date, '00000000'),'YYYYMMDD'),'YYYY-MM-DD') as expiredDate, coalesce(b.download_count, 0) as downCount, ");
            sb.Append("  get_forward_flag(a.trans_seq) as dataForwarded, get_user_info(a.user_seq) as orgUserInfo, a.recv_pos as recvPos, '0' as receiveType ");
            sb.Append("FROM tbl_transfer_req_info a ");
            sb.Append("  LEFT OUTER JOIN view_backup_period b ON (a.trans_seq = b.trans_seq) ");
            sb.Append("WHERE a.user_seq IN (select user_seq from tbl_user_info where user_id = '" + tParam.UserID + "') ");

            if (!(tParam.SearchFromDay.Equals("")) && (tParam.SearchToDay.Equals("")))
            {
                sb.Append("  AND a.request_time >= '" + tParam.SearchFromDay + "'");
            }
            */

        }
    }
}
