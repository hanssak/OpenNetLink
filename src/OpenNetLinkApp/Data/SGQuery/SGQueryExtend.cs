using OpenNetLinkApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using static OpenNetLinkApp.Common.Enums;

namespace OpenNetLinkApp.Data.SGQuery
{
    public class SGQueryExtend
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

            Dictionary<string, string> param = new Dictionary<string, string>() { 
                { "strSysID", strSysID }, { "strUserName", strUserName }, { "strTeamName", strTeamName }, { "strTeamCode", strTeamCode }, { "strApprPos", strApprPos },
            };
            
            return SQLXmlService.Instanse.GetSqlQuery("DeptApprLineSearch", param);
        }

        /// <summary>
        /// 사용자의 유효성을 검사한다.
        /// </summary>
        /// <param name="strUserSeq">사용자리스트(사용자시퀀스 리스트, 구분자(,))</param>
        /// <param name="strTeamCode">팀코드(부서시퀀스)</param>
        /// <param name="bApprPos">결재자권한</param>
        /// <returns>사용자 정보 조회 쿼리</returns>
        public string GetUserConfirm(string strUserSeq, string strTeamCode, bool bApproveProxyRight)
        {

            //일반 사용자도 결재권한을 가진다면 권한 인자 empty로 하여 권한 체크하지 않도록
            string strApprPos = (bApproveProxyRight) ? "" : "1";

            Dictionary<string, string> param = new Dictionary<string, string>() {
                { "strUserSeq", strUserSeq }, { "strTeamCode", strTeamCode }, { "strApprPos", strApprPos }
            };

            return SQLXmlService.Instanse.GetSqlQuery("UserConfirm", param);
        }

        public string GetReceiverSearchQuery(string stSenderId, string strUserName, string strDeptName, string strDeptSeq)
        {
            Dictionary<string, string> param = new Dictionary<string, string>() {
                { "stSenderId", stSenderId }, { "strUserName", strUserName }, { "strDeptName", strDeptName }, { "strDeptSeq", strDeptSeq }
            };

            return SQLXmlService.Instanse.GetSqlQuery("ReceiverSearchQuery", param);
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
            Dictionary<string, string> param = new Dictionary<string, string>() {
                { "bSFM", Convert.ToInt32(bSFM).ToString() }, { "userSeq", userSeq }, { "isDept", Convert.ToInt32(isDept).ToString() }, { "dept", dept }, { "apprName", apprName }
            };

            return SQLXmlService.Instanse.GetSqlQuery("SecurityApprover", param);
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
            Dictionary<string, string> param = new Dictionary<string, string>() {
                { "strUserSeq", strUserSeq }, { "strSystemType", strSystemType }, { "strBlockType", strBlockType }, { "strBlockReason", strBlockReason }
            };

            //바로 암호화 하지 않고 보낼때 암호화
            StringBuilder sb = new StringBuilder();
            SQLXmlService.Instanse.GetSqlQuery("AgentBlock", param, ref sb);
            sb.Replace(Environment.NewLine, "");
            sb.Replace("\t", "");
            sb.Replace("\\n", "\n");
            return sb.ToString();
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

            Dictionary<string, string> param = new Dictionary<string, string>() {
                { "strSystem", strSystem }, { "strUserSeq", strUserSeq }, { "strDate", strDate }
            };

            return SQLXmlService.Instanse.GetSqlQuery("DayFileTransInfo", param);
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

            Dictionary<string, string> param = new Dictionary<string, string>() {
                { "strUserSeq", strUserSeq }, { "strDate", strDate }, { "strSystem", strSystem }
            };
            
            return SQLXmlService.Instanse.GetSqlQuery("DayClipboardInfo", param);
        }

        /// <summary>
        /// 파일 내부검사 설정 정보를 조회한다.
        /// </summary>
        /// <returns>쿼리문</returns>
        public string GetUnzipCheckDepth()
        {
            Dictionary<string, string> param = new Dictionary<string, string>() {
            };

            return SQLXmlService.Instanse.GetSqlQuery("UnzipCheckDepth", param);
        }

        /// <summary>
        /// 대쉬보드 전송요청 카운트를 조회한다.
        /// </summary>
        /// <param name="strUserSeq">사용자 Seq</param>
        /// <param name="strFromDate">올해정보</param>
        /// <param name="strToDate">쿼리문</param>
        /// <returns></returns>
        public string GetDashboardTransReqCountQuery(string strUserSeq, string strFromDate, string strToDate)
        {
            Dictionary<string, string> param = new Dictionary<string, string>() {
                { "strUserSeq", strUserSeq }, { "strFromDate", strFromDate }, { "strToDate", strToDate }
            };

            return SQLXmlService.Instanse.GetSqlQuery("DashboardTransReqCountQuery", param);
        }

        /// <summary>
        /// 패스워드 최종 변경 날짜를 리턴한다.
        /// </summary>
        /// <param name="strUserSeq">사용자 Seq</param>
        /// <returns>쿼리문</returns>
        public string GetPasswdChgDay(string strUserSeq)
        {
            Dictionary<string, string> param = new Dictionary<string, string>() {
                { "strUserSeq", strUserSeq }
            };

            return SQLXmlService.Instanse.GetSqlQuery("PasswdChgDay", param);
        }

        /// <summary>
        /// 공지사항을 조회하는 쿼리를 리턴한다.
        /// </summary>
        /// <param name="strUserID">사용자 ID</param>
        /// <param name="strPreDate">날짜 </param>
        /// <returns>쿼리문</returns>
        public string GetSGNotify(string strUserID, string strPreDate = "")
        {
            Dictionary<string, string> param = new Dictionary<string, string>() {
                { "strUserID", strUserID }, { "strPreDate", strPreDate }
            };

            return SQLXmlService.Instanse.GetSqlQuery("SGNotify", param);
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
                Dictionary<string, string> param = new Dictionary<string, string>() {
                { "strUserList", strUserList }
            };

                return SQLXmlService.Instanse.GetSqlQuery("ApproveAfterCount", param); 
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
            Dictionary<string, string> param = new Dictionary<string, string>() {
                { "strNotifySeq", strNotifySeq }, { "strUserID", strUserID }, { "strNomore", strNomore }
            };

            return SQLXmlService.Instanse.GetSqlQuery("SGNotifyStatus", param);
        }

        /// <summary>
        /// systemenv 에서 지정한 Tag의 data들을 가져오는 함수
        /// </summary>
        /// <returns></returns>
        public string GetsystemEnvDataS(string strTagNames)
        {
            Dictionary<string, string> param = new Dictionary<string, string>() {
                { "strTagNames", strTagNames }
            };

            return SQLXmlService.Instanse.GetSqlQuery("systemEnvDataS", param);
        }

        /// <summary>
        /// CLIENT_ZIP_DEPTH 정보 가져오는 Query
        /// </summary>
        /// <returns></returns>
        public string GetZipDepthSQLsystemEnv()
        {
            Dictionary<string, string> param = new Dictionary<string, string>() {
            };

            return SQLXmlService.Instanse.GetSqlQuery("ZipDepthSQLsystemEnv", param);
        }

        public string GetDeptInfo(string deptName)
        {
            Dictionary<string, string> param = new Dictionary<string, string>() {
                { "deptName", deptName}
            };

            return SQLXmlService .Instanse.GetSqlQuery("DeptInfo", param);
        }


        /// <summary>
        /// TRANSFER SEQ를 가지고 개인정보로그를 가져오기
        /// </summary>
        /// <param name="transSeq"></param>
        /// <returns></returns>
        public static string GetTransferInfoPrivacy(string transSeq)
        {
            Dictionary<string, string> param = new Dictionary<string, string>() {
                { "transSeq", transSeq}
            };

            return SQLXmlService.Instanse.GetSqlQuery("TransferInfoPrivacy", param);
        }

        public static string GetSkipFileListSet(string systemid, string g_strFileName, long lfileSize, string strReasonData, string userSeq, string strsha384, int g_ninterLockFlag)
        {
            Dictionary<string, string> param = new Dictionary<string, string>() {
                { "systemid", systemid}, { "g_strFileName", g_strFileName}, { "lfileSize", lfileSize.ToString()}, { "strReasonData", strReasonData}, { "userSeq", userSeq}, { "strsha384", strsha384}, { "g_ninterLockFlag", g_ninterLockFlag.ToString()}
            };

            return SQLXmlService.Instanse.GetSqlQuery("SkipFileListSet", param);
        }

        public static string GetSkipFileCount(string userSeq)
        {
            Dictionary<string, string> param = new Dictionary<string, string>() {
                { "userSeq", userSeq}
            };

            return SQLXmlService.Instanse.GetSqlQuery("SkipFileCount", param);
        }

        public static string GetSkipFileList(string userSeq, int pageListCount, int viewPageNo)
        {
            Dictionary<string, string> param = new Dictionary<string, string>() {
                { "userSeq", userSeq}, { "pageListCount", pageListCount.ToString()}, { "viewPageNo", viewPageNo.ToString()}
            };

            return SQLXmlService.Instanse.GetSqlQuery("SkipFileList", param);
        }



        /// <summary>
        /// tbl_file_ole_mimetype 테이블 조회하기
        /// </summary>
        /// <returns></returns>
        public static string GetOLEMimeList()
        {
            Dictionary<string, string> param = new Dictionary<string, string>() {
            };

            return SQLXmlService.Instanse.GetSqlQuery("OLEMimeList", param);
        }

        public static string GetFileExceptionCancel(string userSeq, List<string> list)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (string str in list)
            {
                Dictionary<string, string> subParam = new Dictionary<string, string>() {
                    { "userSeq", userSeq }, { "strFileSeq", str}
                    
                };
                StringBuilder subSb = new StringBuilder();
                SQLXmlService.Instanse.GetSqlQuery("FileExceptionCancel", subParam, ref subSb);
                subSb.Replace(Environment.NewLine, "");
                subSb.Replace("\t", "");
                subSb.Replace("\\n", "\n");
                if (i == 0)
                    sb.Append("Hsck_Transaction:" + subSb.ToString());
                else
                    sb.Append(subSb.ToString());

                i++;
                subSb.Clear();
            }
            sb.Replace(Environment.NewLine, "");
            sb.Replace("\t", "");
            sb.Replace("\\n", "\n");
            string sql = sb.ToString();
            return HsNetWorkSG.SGCrypto.AESEncrypt256WithDEK(ref sql);
        }

        public static string SearchURLList(string strSearchURL)
        {
            Dictionary<string, string> param = new Dictionary<string, string>()
            {
                {"strSearchURL", strSearchURL }
            };

            return SQLXmlService.Instanse.GetSqlQuery("SearchURLList", param);
        }

        public static string AgentHashFailLog(string systemId, string version, string strUserID, string ipAddr)
        {
            Dictionary<string, string> param = new Dictionary<string, string>()
            {
                {"systemId", systemId }, {"version", version }, {"strUserID", strUserID }, {"ipAddr", ipAddr }
            };

            return SQLXmlService.Instanse.GetSqlQuery("AgentHashFailLog", param);
        }
    }
}
