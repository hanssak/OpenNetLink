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
        ~SGQueryExtend()
        {

        }
        /**
        *@breif 현재 조회되는 부서의 결재자 리스트를 반환한다.
        *@param strSysID 내/외부 구분( I : 내부, E : 외부 ) 
        *@param strUserName 사용자 이름
        *@param strTeamName 팀이름
        *@param strTeamCode 팀코드
        *@param bApprPos 결재자 검색인지 아닌지 구분자 ( true : 결재자만 검색, false : 해당 부서의 모든 사용자 검색)
        *@return 쿼리문
        */
        public string GetDeptApprLineSearch(string strSysID, string strUserName, string strTeamName, string strTeamCode, bool bApprPos)
        {
            string strApprPos = "0";
            if (bApprPos)
                strApprPos = "1";

            string strQuery = "";
            strQuery = String.Format("SELECT * FROM FUNC_USERINFO_SEARCH('{0}', '{1}', '{2}', '{3}', '{4}')", strUserName, strTeamName, strTeamCode, strApprPos, strSysID);
            return strQuery;
        }

        /**
        *@breif 파일 추가 시 차단된 이력을 서버로 전송한다.
        *@param strUserSeq 사용자Seq
        *@param strSystemType 내/외부 구분( I : 내부, E : 외부 )
        *@param strBlockType 차단 타입
        *@param strBlockReason 차단 사유
        *@return 쿼리문
        */
        public string GetAgentBlock(string strUserSeq, string strSystemType, string strBlockType, string strBlockReason)
        {
            string strQuery = "";
            strQuery = String.Format("insert into tbl_agent_block values({0},'{1}','{2}','{3}',Now())\n", strUserSeq, strSystemType, strBlockType, strBlockReason);
            return strQuery;
        }

        /**
        *@breif 일일 전송한 파일 사이즈 및 횟수를 조회하는 쿼리를 반환한다.
        *@param bSystem 시스템 정보(true:업무망, false:인터넷망)
        *@param strUserSeq 사용자Seq
        *@param time 날짜 및 시간
        *@param strConnNetwork 접속망에대한 정보 (0:업무-인터넷망 1:운영-업무망)
        *@return 쿼리문
        */

        public string GetDayFileTransInfo(bool bSystem, string strUserSeq, string strDate, string strConnNetwork)
        {
            string strSystem = "I";
            if (!bSystem)
                strSystem = "E";
            strSystem = strSystem + strConnNetwork;

            string strQuery = "SELECT U.USER_ID, SUM(F.FILE_SIZE) AS FS, COUNT(*) AS CNT ";
            strQuery += "FROM ( ";
            strQuery += "SELECT 'H' AS TPOS, TRANS_SEQ, REQUEST_TIME, USER_SEQ, TRANS_FLAG, RECV_FLAG, PCTRANS_FLAG,APPROVE_FLAG, SYSTEM_ID ";
            strQuery += "FROM TBL_TRANSFER_REQ_HIS H WHERE TRANS_SEQ BETWEEN '##DATE##0000000000' AND '##DATE##9999999999' ";
            strQuery += "UNION ALL \n";
            strQuery += "SELECT 'C' AS TPOS, TRANS_SEQ, REQUEST_TIME, USER_SEQ, TRANS_FLAG, RECV_FLAG, PCTRANS_FLAG,APPROVE_FLAG, SYSTEM_ID \n";
            strQuery += "FROM TBL_TRANSFER_REQ_INFO T WHERE TRANS_SEQ BETWEEN '##DATE##0000000000' AND '##DATE##9999999999' ";
            strQuery += ") T ";
            strQuery += ", TBL_USER_INFO U ";
            strQuery += ", ( \n";
            strQuery += "SELECT TRANS_SEQ, SUM(F.FILE_SIZE) AS FILE_SIZE ";
            strQuery += "FROM TBL_FILE_LIST_HIS F WHERE FILE_SEQ BETWEEN '##DATE##0000000000' AND '##DATE##9999999999' ";
            strQuery += "GROUP BY F.TRANS_SEQ  ";
            strQuery += ") F ";
            strQuery += "WHERE T.TRANS_SEQ=F.TRANS_SEQ ";
            strQuery += "AND T.REQUEST_TIME BETWEEN '##DATE##0000' AND '##DATE##235959' ";
            strQuery += "AND U.USER_SEQ=T.USER_SEQ ";
            strQuery += "AND U.USER_SEQ='##USERSEQ##' ";
            strQuery += "AND FUNC_TRANSSTATUS(T.TRANS_FLAG, T.RECV_FLAG, T.PCTRANS_FLAG) NOT IN ('C', 'F') ";
            strQuery += "AND T.APPROVE_FLAG !='3' ";
            strQuery += "AND SUBSTRING(T.SYSTEM_ID, 1, 2)='##SYSID##' ";
            strQuery += "GROUP BY U.USER_ID ";

            strQuery = strQuery.Replace("##USERSEQ##", strUserSeq);
            strQuery = strQuery.Replace("##DATE##", strDate);
            strQuery = strQuery.Replace("##SYSID##", strSystem);

            return strQuery;
        }
        /**
        *@breif 일일클립보드 전송한 사이즈 및 횟수를 조회하는 쿼리를 반환한다.
        *@param bSystem 시스템 정보(true:업무망, false:인터넷망)
        *@param strUserSeq 사용자Seq
        *@param time 날짜 및 시간
        *@param strConnNetwork 접속망에대한 정보 (0:업무-인터넷망 1:운영-업무망)
        *@return 쿼리문
        */
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

        /**
        *@breif Zip 파일 내부검사 설정 정보를 조회한다.
        *@return 쿼리문
        */
        public string GetUnzipCheckDepth()
        {
            string strQuery = "SELECT CAST(SUBSTRING(SYSTEM_ID, 1, 1)||'_'||TAG AS VARCHAR) TAG, TAG_VALUE ";
            strQuery += "FROM TBL_SYSTEM_ENV ";
            strQuery += "WHERE SUBSTRING(SYSTEM_ID, 4, 1)='1' ";
            strQuery += "	AND TAG IN ('CLIENT_ZIP_DEPTH') ";
            strQuery += "ORDER BY SYSTEM_ID DESC";
            return strQuery;
        }

        /**
        *@breif 대쉬보드에서 전송요청,승인대기,승인,반려 카운트를 조회한다.
        *@param strUserSeq 사용자 Seq
        *@param strDate 현재 날짜
        *@return 쿼리문
        */
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
    }
}
