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
            strQuery = String.Format("SELECT * FROM FUNC_USERINFO_SEARCH('{0}', '{1}', '{2}', '{3}', '{4}')", strUserName, strTeamName, strTeamCode,strApprPos,strSysID);
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
            strQuery = String.Format("insert into tbl_agent_block values({0},'{1}','{2}','{3}',Now())\n", strUserSeq,strSystemType, strBlockType, strBlockReason);
            return strQuery;
        }

        /**
        *@breif 일일 전송한 파일 사이즈 및 횟수를 조회하는 쿼리를 반환한다.
        *@param bSystem 시스템 정보(true:업무망, false:인터넷망)
        *@param strUserSeq 사용자Seq
        *@param time 날짜 및 시간
        *@return 쿼리문
        */
        /*
        public string GetDayFileTransferInfo(bool bSystem, string strUserSeq, DateTime time, string strConnNetwork)
        {
            string strSystem = "I";
            if (!bSystem)
                strSystem = "E";
            strSystem = strSystem + strConnNetwork;

            string strYear = time.Year.ToString();
            string strMonth = time.Month.ToString();
            string strDay = time.Day.ToString();
            string strTime = strYear + strMonth + strDay;

        }
        */
        
    }
}
