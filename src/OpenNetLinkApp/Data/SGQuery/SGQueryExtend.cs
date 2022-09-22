using System;
using System.Collections.Generic;
using System.Text;
using static OpenNetLinkApp.Common.Enums;

namespace OpenNetLinkApp.Data.SGQuery
{
    class SGQueryExtend
    {
        public SGQueryExtend()
        {

        }
        ~SGQueryExtend()
        {

        }

        /// <summary>
        /// 현재 조회되는 부서의 결재자 리스트를 반환한다.
        /// </summary>
        /// <param name="strSysID">내/외부 구분( I : 내부, E : 외부 ) </param>
        /// <param name="strUserName">사용자 이름</param>
        /// <param name="strTeamName">팀이름</param>
        /// <param name="strTeamCode">팀코드</param>
        /// <param name="bApprPos">결재자 검색인지 아닌지 구분자 ( true : 결재자만 검색, false : 해당 부서의 모든 사용자 검색)</param>
        /// <returns>쿼리문</returns>
        public string GetDeptApprLineSearch(string strSysID, string strUserName, string strTeamName, string strTeamCode, bool bApprPos)
        {
            string strApprPos = "0";
            if (bApprPos)
                strApprPos = "1";

            string strQuery = "";
            strQuery = String.Format("SELECT * FROM FUNC_USERINFO_SEARCH('{0}', '{1}', '{2}', '{3}', '{4}')", strUserName, strTeamName, strTeamCode, strApprPos, strSysID);
            return strQuery;
        }

        /// <summary>
        /// 사용자의 유효성을 검사한다.
        /// </summary>
        /// <param name="strUserSeq">사용자리스트(사용자시퀀스 리스트, 구분자(,))</param>
        /// <param name="strTeamCode">팀코드(부서시퀀스)</param>
        /// <param name="bApprPos">결재자권한</param>
        /// <returns>사용자 정보 조회 쿼리</returns>
        public string GetUserConfirm(string strUserSeq, string strTeamCode, bool bApprPos)
        {
            string strApprPos = "0";
            if (bApprPos)
                strApprPos = "1";

            string strQuery = "";
            strQuery = String.Format("SELECT * FROM func_nl_getuserconfirm_v3('{0}', '{1}', '{2}')", strUserSeq, strTeamCode, strApprPos);
            return strQuery;
        }

        public string GetReceiverSearchQuery(string stSenderId, string strUserName, string strDeptName)
        {
            string stQuery = string.Empty;
            stQuery += " select a.user_id, a.user_name, b.dept_seq, b.dept_name, a.user_position, a.user_rank, a.part_owner, ";
            stQuery += " a.apprpos, a.user_seq, a.dlp_approve ";
            stQuery += " from tbl_user_info a ";
            stQuery += " join tbl_dept_info b on b.dept_seq = a.dept_seq";
            stQuery += " where a.user_id != '" + stSenderId + "' ";
            if( strUserName.Length > 0)
                stQuery += " and a.user_name like '%%" + strUserName + "%%'";
            
            if ( strDeptName.Length > 0)
                stQuery += " and b.dept_name like '%%" + strDeptName + "%%'";
            stQuery += " limit 100 ";
            return stQuery;
        }
        /// <summary>
        /// 보안결재자 조회
        /// </summary>
        /// <param name="bSFM">대결재 여부</param>
        /// <param name="userSeq">자신 SEQ</param>
        /// <param name="isDept">자신이 속한 부서만 조회여부</param>
        /// <param name="dept">부서명</param>
        /// <param name="apprName">승인자명</param>
        /// <returns></returns>
        public string GetSecurityApprover(bool bSFM, string userSeq, bool isDept, string dept, string apprName)
        {
            string stQuery = $@"
SELECT AA.USER_ID, AA.USER_NAME,AA.DEPT_SEQ, BB.DEPT_NAME,
AA.USER_POSITION, AA.USER_RANK, AA.APPRPOS, AA.APPRPOS_EX, AA.USER_SEQ, AA.DLP_APPROVE
FROM TBL_USER_INFO AA, TBL_DEPT_INFO BB
WHERE USER_SEQ IN (
SELECT USER_SEQ AS USER_SEQ FROM TBL_USER_INFO
WHERE (DLP_APPROVE = '1' AND USER_SEQ != {userSeq})";
            if ( bSFM )
            {
                stQuery += $@"
UNION
SELECT SFM_USER_SEQ AS USER_SEQ FROM TBL_USER_SFM A
WHERE A.USER_SEQ IN (SELECT USER_SEQ FROM TBL_USER_INFO WHERE DLP_APPROVE = '1'  AND USER_SEQ != {userSeq}) ";
            }

            if (isDept)
            {
                stQuery += $@"
)
AND AA.DEPT_SEQ = BB.DEPT_SEQ AND AA.USE_STATUS = '1' AND AA.ACCOUNT_EXPIRES > TO_CHAR(NOW(), 'YYYYMMDD')
AND BB.DEPT_NAME = '{dept}' AND AA.USER_NAME LIKE '%%{apprName}%%'
";
            }
            else
            {
                stQuery += $@"
)
AND AA.DEPT_SEQ = BB.DEPT_SEQ AND AA.USE_STATUS = '1' AND AA.ACCOUNT_EXPIRES > TO_CHAR(NOW(), 'YYYYMMDD')
AND BB.DEPT_NAME LIKE '%%{dept}%%' AND AA.USER_NAME LIKE '%%{apprName}%%'
";
            }
            return stQuery;
        }

        //public string GetSecurityApproverMyDept(bool bSFM, string userSeq)
        //{
        //    string stQuery = "";
        //    stQuery = "SELECT AA.user_id, AA.user_name,AA.dept_seq, BB.dept_name,";
        //    stQuery += " AA.user_position, AA.user_rank, AA.apprpos, AA.apprpos_ex, AA.user_seq, AA.dlp_approve ";
        //    stQuery += " FROM tbl_user_info AA, tbl_dept_info BB ";
        //    stQuery += " WHERE user_seq in ( ";
        //    stQuery += " select user_seq AS user_seq from tbl_user_info ";
        //    stQuery += (" where (dlp_approve = '1' AND user_seq != " + userSeq + ")");
        //    if (bSFM)
        //    {
        //        stQuery += " UNION ";
        //        stQuery += " select sfm_user_seq AS user_seq from tbl_user_sfm a ";
        //        stQuery += " WHERE ";
        //        stQuery += " ( ";
        //        stQuery += "    a.user_seq in ";
        //        stQuery += ("    (SELECT user_seq from tbl_user_info WHERE dlp_approve = '1'  AND user_seq != " + userSeq + ") ");
        //        stQuery += " ) ";
        //    }
        //    stQuery += " ) ";
        //    stQuery += " AND BB.dept_seq = (SELECT CC.dept_seq FROM tbl_user_info CC WHERE CC.user_seq='" + userSeq + "') AND AA.dept_seq = BB.dept_seq AND AA.use_status = '1' and AA.account_expires > TO_CHAR(NOW(), 'YYYYMMDD') ";
        //    return stQuery;
        //}

        /// <summary>
        /// 파일 추가 시 차단된 이력을 서버로 전송한다.
        /// </summary>
        /// <param name="strUserSeq">사용자Seq</param>
        /// <param name="strSystemType">내/외부 구분( I : 내부, E : 외부 )</param>
        /// <param name="strBlockType">차단 타입</param>
        /// <param name="strBlockReason">차단 사유</param>
        /// <returns>쿼리문</returns>
        public string GetAgentBlock(string strUserSeq, string strSystemType, string strBlockType, string strBlockReason)
        {
            string strQuery = "";
            strQuery = String.Format("insert into tbl_agent_block values({0},'{1}','{2}','{3}',Now())\n", strUserSeq, strSystemType, strBlockType, strBlockReason);
            return strQuery;
        }

        /// <summary>
        /// 일일 전송한 파일 사이즈 및 횟수를 조회하는 쿼리를 반환한다.
        /// </summary>
        /// <param name="bSystem">시스템 정보(true:업무망, false:인터넷망)</param>
        /// <param name="strUserSeq">사용자Seq</param>
        /// <param name="strDate">날짜 및 시간</param>
        /// <param name="strConnNetwork">접속망에대한 정보 (0:업무-인터넷망 1:운영-업무망)</param>
        /// <returns></returns>
        public string GetDayFileTransInfo(bool bSystem, string strUserSeq, string strDate, string strConnNetwork)
        {
            string strSystem = "I";
            if (!bSystem)
                strSystem = "E";
            strSystem = strSystem + strConnNetwork;

            string strQuery = $@"
SELECT U.USER_ID, SUM(F.FILE_SIZE) AS FS, COUNT(*) AS CNT
FROM ( 
SELECT 'H' AS TPOS, TRANS_SEQ, REQUEST_TIME, USER_SEQ, TRANS_FLAG, RECV_FLAG, PCTRANS_FLAG,APPROVE_FLAG, SYSTEM_ID
FROM TBL_TRANSFER_REQ_HIS H 
WHERE TRANS_SEQ BETWEEN '{strDate}0000000000' AND '{strDate}9999999999'
AND DATA_TYPE = 0
UNION ALL
SELECT 'C' AS TPOS, TRANS_SEQ, REQUEST_TIME, USER_SEQ, TRANS_FLAG, RECV_FLAG, PCTRANS_FLAG,APPROVE_FLAG, SYSTEM_ID
FROM TBL_TRANSFER_REQ_INFO T 
WHERE TRANS_SEQ BETWEEN '{strDate}0000000000' AND '{strDate}9999999999'
AND DATA_TYPE = 0
) T 
, TBL_USER_INFO U
, (
SELECT TRANS_SEQ, SUM(F.FILE_SIZE) AS FILE_SIZE
FROM TBL_FILE_LIST_HIS F WHERE FILE_SEQ BETWEEN '{strDate}0000000000' AND '{strDate}9999999999'
GROUP BY F.TRANS_SEQ
) F
WHERE T.TRANS_SEQ=F.TRANS_SEQ
AND T.REQUEST_TIME BETWEEN '{strDate}0000' AND '{strDate}235959'
AND U.USER_SEQ=T.USER_SEQ
AND U.USER_SEQ='{strUserSeq}'
AND FUNC_TRANSSTATUS(T.TRANS_FLAG, T.RECV_FLAG, T.PCTRANS_FLAG) NOT IN ('C', 'F')
AND T.APPROVE_FLAG !='3'
AND SUBSTRING(T.SYSTEM_ID, 1, 2)='{strSystem}'
GROUP BY U.USER_ID ";
            
            return strQuery;
        }

        /// <summary>
        /// 일일클립보드 전송한 사이즈 및 횟수를 조회하는 쿼리를 반환한다.
        /// </summary>
        /// <param name="bSystem">시스템 정보(true:업무망, false:인터넷망)</param>
        /// <param name="strUserSeq">사용자Seq</param>
        /// <param name="strDate">날짜 및 시간</param>
        /// <param name="strConnNetwork">접속망에대한 정보 (0:업무-인터넷망 1:운영-업무망)</param>
        /// <returns></returns>
        public string GetDayClipboardInfo(bool bSystem, string strUserSeq, string strDate, string strConnNetwork)
        {
            string strSystem = "I";
            if (!bSystem)
                strSystem = "E";
            strSystem = strSystem + strConnNetwork;

            string strQuery = "SELECT USER_ID,  SUM(DATA_SIZE) AS DATA_SIZE, COUNT(*) AS CNT ";
            strQuery += "FROM TBL_CLIPBOARD_HIS ";
            strQuery += "WHERE WORK_ID BETWEEN '##DATE##0000000000' AND '##DATE##9999999999' ";
            strQuery += "AND SUBSTRING(SYSTEM_ID, 1,2)='##SYSID##' ";
            strQuery += "AND DATA_TYPE IN ('1', '2', '4') ";
            strQuery += "AND USER_ID = (SELECT USER_ID FROM TBL_USER_INFO WHERE USER_SEQ='##USERSEQ##')";
            strQuery += "GROUP BY USER_ID";

            strQuery = strQuery.Replace("##USERSEQ##", strUserSeq);
            strQuery = strQuery.Replace("##DATE##", strDate);
            strQuery = strQuery.Replace("##SYSID##", strSystem);
            return strQuery;
        }

        /// <summary>
        /// 파일 내부검사 설정 정보를 조회한다.
        /// </summary>
        /// <returns>쿼리문</returns>
        public string GetUnzipCheckDepth()
        {
            string strQuery = "SELECT CAST(SUBSTRING(SYSTEM_ID, 1, 1)||'_'||TAG AS VARCHAR) TAG, TAG_VALUE ";
            strQuery += "FROM TBL_SYSTEM_ENV ";
            strQuery += "WHERE SUBSTRING(SYSTEM_ID, 4, 1)='1' ";
            strQuery += "	AND TAG IN ('CLIENT_ZIP_DEPTH') ";
            strQuery += "ORDER BY SYSTEM_ID DESC";
            return strQuery;
        }

        /// <summary>
        /// 대쉬보드에서 전송요청,승인대기,승인,반려 카운트를 조회한다.
        /// </summary>
        /// <param name="strUserSeq">사용자 Seq</param>
        /// <param name="strDate">현재 날짜</param>
        /// <returns>쿼리문</returns>
        public string GetDashboardCountQuery(string strUserSeq,string strDate)
        {
            string strQuery = "SELECT (select A.cnt + B.cnt FROM(select COUNT(*) cnt from tbl_transfer_req_his where user_seq = '##USERSEQ##' and request_time BETWEEN '19900101000000' AND '##DATE##235959') A, ";
            strQuery += "(select COUNT(*) cnt from tbl_transfer_req_his where user_seq = '##USERSEQ##' and request_time BETWEEN '19900101000000' AND '##DATE##235959') B) AS reqcount, ";
            strQuery += "(select A.cnt + B.cnt FROM (select COUNT(*) cnt from tbl_transfer_req_his where approve_flag = '1' and trans_flag != '5' and user_seq = '##USERSEQ##' and request_time BETWEEN '19900101000000' AND '##DATE##235959') A, ";
            strQuery += "( select COUNT(*) cnt from tbl_transfer_req_his where approve_flag='1' and trans_flag!='5' and user_seq = '##USERSEQ##' and request_time BETWEEN '19900101000000' AND '##DATE##235959') B) AS readyapprove, ";
            strQuery += "(select A.cnt + B.cnt FROM (select COUNT(*) cnt from tbl_transfer_req_his where approve_flag = '2' and user_seq = '##USERSEQ##' and request_time BETWEEN '19900101000000' AND '##DATE##235959') A, ";
            strQuery += "(select COUNT(*) cnt from tbl_transfer_req_his where approve_flag='2' and user_seq = '##USERSEQ##' and request_time BETWEEN '19900101000000' AND '##DATE##235959') B ) AS approve, ";
            strQuery += "(select A.cnt + B.cnt FROM (select COUNT(*) cnt from tbl_transfer_req_his where approve_flag='3' and user_seq='##USERSEQ##' and request_time BETWEEN '19900101000000' AND '##DATE##235959') A, ";
            strQuery += "(select COUNT(*) cnt from tbl_transfer_req_his where approve_flag='3' and user_seq = '##USERSEQ##' and request_time BETWEEN '19900101000000' AND '##DATE##235959') B ) AS deny";
            strQuery = strQuery.Replace("##USERSEQ##", strUserSeq);
            strQuery = strQuery.Replace("##DATE##", strDate);
            return strQuery;
        }

        /// <summary>
        /// 대쉬보드 전송요청 카운트를 조회한다.
        /// </summary>
        /// <param name="strUserSeq">사용자 Seq</param>
        /// <param name="strFromDate">올해정보</param>
        /// <param name="strToDate">쿼리문</param>
        /// <returns></returns>
        public string GetDashboardTransReqCountQuery(string strUserSeq, string strFromDate,string strToDate)
        {
            string strQuery = "SELECT (select A.cnt + B.cnt FROM(select COUNT(*) cnt from tbl_transfer_req_his where user_seq = '##USERSEQ##' and request_time BETWEEN '##FROMDATE##' AND '##TODATE##') A, ";
            strQuery += "(select COUNT(*) cnt from tbl_transfer_req_his where user_seq = '##USERSEQ##' and request_time BETWEEN '##FROMDATE##' AND '##TODATE##') B) AS reqcount";
            strQuery = strQuery.Replace("##USERSEQ##", strUserSeq);
            //strQuery = strQuery.Replace("##DATE##", strDate);
            strQuery = strQuery.Replace("##FROMDATE##", strFromDate);
            strQuery = strQuery.Replace("##TODATE##", strToDate);
            return strQuery;
        }

        /// <summary>
        /// 패스워드 최종 변경 날짜를 리턴한다.
        /// </summary>
        /// <param name="strUserSeq">사용자 Seq</param>
        /// <returns>쿼리문</returns>
        public string GetPasswdChgDay(string strUserSeq)
        {
            string strQuery = "select passwdexpired from tbl_user_info where user_seq='##USERSEQ##'";
            strQuery = strQuery.Replace("##USERSEQ##", strUserSeq);
            return strQuery;
        }

        /// <summary>
        /// 공지사항을 조회하는 쿼리를 리턴한다.
        /// </summary>
        /// <param name="strUserID">사용자 ID</param>
        /// <param name="strPreDate">날짜 </param>
        /// <returns>쿼리문</returns>
        public string GetSGNotify(string strUserID, string strPreDate="")
        {
            string strQuery;
            strQuery=String.Format("SELECT * FROM FUNC_NL_BOARDNOTIFY('{0}', '{1}')", strUserID, strPreDate);
            return strQuery;
        }

        /// <summary>
        /// 사후 결재 결재자의 결재 리스트 Count(경고, 제한) 수 가져오기
        /// </summary>
        /// <param name="strUserList"></param>
        /// <returns></returns>
        public string GetApproveAfterCount(string strUserList, EnumApproveTime enumApproveTime)
        {
            string sql = String.Empty;

            if (enumApproveTime == EnumApproveTime.After)
            {
                sql = $@"
SELECT * FROM FUNC_NL_GETAFTERAPPROVEWAITUSERCOUNT('{strUserList}')
";
            }
            return sql;
        }

        /// <summary>
        /// 공지사항의 읽은 상태를 변경하는 쿼리를 반환한다.
        /// </summary>
        /// <param name="strNotifySeq">공지사항 시퀀스</param>
        /// <param name="strUserID">사용자 아이디</param>
        /// <param name="strNomore"></param>
        /// <returns>공지사항의 읽은 상태를 변경하는 쿼리</returns>
        public string GetSGNotifyStatus(string strNotifySeq, string strUserID, string strNomore)
        {
            string strQuery;
            strQuery = String.Format("SELECT * FROM FUNC_NL_BOARDREADSTATUS({0}, '{1}', '{2}')", strNotifySeq, strUserID, strNomore);

            return strQuery;
        }

        /// <summary>
        /// CLIENT_ZIP_DEPTH 정보 가져오는 Query
        /// </summary>
        /// <returns></returns>
        public string GetZipDepthSQLsystemEnv()
        {
            string strQuery = "SELECT CAST(SUBSTRING(SYSTEM_ID, 1, 1)||'_'||TAG AS VARCHAR) TAG, TAG_VALUE FROM TBL_SYSTEM_ENV WHERE SUBSTRING(SYSTEM_ID, 4, 1)='1' AND TAG IN ('CLIENT_ZIP_DEPTH') ORDER BY SYSTEM_ID DESC";
            return strQuery;
        }
        /// <summary>
        /// TRANSFER SEQ를 가지고 개인정보로그를 가져오기
        /// </summary>
        /// <param name="transSeq"></param>
        /// <returns></returns>
        public static string GetTransferInfoPrivacy(string transSeq)
        {
            string sql = $@"
SELECT A.TRANS_SEQ, A.DATA_TYPE,B.FILE_NAME, B.FILE_SIZE, B.FILE_KIND, B.DLP, C.*
FROM TBL_TRANSFER_REQ_INFO A
INNER JOIN TBL_FILE_LIST_HIS B ON A.TRANS_SEQ = B.TRANS_SEQ
INNER JOIN TBL_PRIVACY_HIS C ON B.FILE_SEQ = C.FILE_SEQ AND B.TRANS_SEQ = C.TRANS_SEQ
WHERE A.TRANS_SEQ = '{transSeq}'
";
            return sql;
        }


    }
}
