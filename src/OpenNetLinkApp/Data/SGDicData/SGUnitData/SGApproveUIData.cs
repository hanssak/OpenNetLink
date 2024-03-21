using OpenNetLinkApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HsNetWorkSGData;
using HsNetWorkSG;
using OpenNetLinkApp.Common;
using System.Runtime.InteropServices;
using Serilog;
using AgLogManager;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    public class SGApproveUIData
    {
        /// <summary>
        /// 해당 객체가 사용되는 조회 UI 타입
        /// </summary>
        public enum UIType
        {
            Common,
            //Common_PopUp,
            Security,
            //Security_PopUp,
            Clipboard,
            //Clipboard_PopUp,            
            Mail,
            MailSeciurity,
        
        }


        public class ApprovalInfo
        {
            public ApprovalInfo(string userSeq, string userID, string userName, string approveStatusCode, string approveStatusName, string approveReason, string approveTime)
            {
                UserSeq = userSeq;
                UserID = userID;
                UserName = userName;
                ApproveStatusCode = approveStatusCode;
                ApproveStatusName = approveStatusName;
                ApproveReason = approveReason;
                ApproveTime = approveTime;
            }

            public string UserSeq { get; set; }
            public string UserID { get; set; }
            public string UserName { get; set; }
            public string ApproveStatusCode { get; set; }
            public string ApproveStatusName { get; set; }
            public string ApproveReason { get; set; }
            public string ApproveTime { get; set; }
        }

        XmlConfService xmlConf;
        UIType DisplayUIType;

        private SGApproveUIData() { }
        /// <summary>
        /// 각 결재 화면 별, 전송 건에 대한 정보를 Name 등으로 Converting 하여 표시하는 Class
        /// </summary>
        /// <param name="getUIType"></param>
        public SGApproveUIData(UIType getUIType)
        {
            xmlConf = new XmlConfService();
            DisplayUIType = getUIType;
        }

        /// <summary>
        /// 결재 정보(선결,후결)
        /// <para>"trans_req", "approval_proc_type"</para>
        /// </summary>
        /// <param name="selectedInfo"></param>
        /// <returns>pre/post</returns>
        public string GetApprKindCode(object selectedInfo) => selectedInfo.GetTagDataString("trans_req", "approval_proc_type");

        /// <summary>
        /// 결재 정보(선결,후결)
        /// <para>"trans_req", "approval_proc_type"</para>
        /// </summary>
        /// <param name="selectedInfo"></param>
        /// <returns></returns>
        public string GetApprKind(object selectedInfo)
        {
            string processType = selectedInfo.GetTagDataString("trans_req", "approval_proc_type");
            if (processType == "pre")    //사전
                return xmlConf.GetTitle("T_COMMON_APPROVE_BEFORE");        // 선결
            else if (processType == "post")
                return xmlConf.GetTitle("T_COMMON_APPROVE_AFTER");        // 후결
            else
                return "-";
        }
        /**
         * 
		 * @breif 개인정보 검출 상태 정보를 반환한다.
		 * @return 개인정보 검출 상태 (미사용,포함,미포함,검출불가)
		 */
        public string GetDLP(object selectedInfo)
        {

            //Todo 고도화 - 개인정보 검출(또는 검사결과에 대한 정보) 필요함.
            List<object> scanList = selectedInfo.GetTagDataObjectList("scan_list");
            foreach (object scan in scanList)
            {
                if (scan.GetTagDataObject("type").ToString().ToUpper() != "DLP")
                    continue;

                string state = scan.GetTagDataObject("result", "scan_state").ToString();
                switch (state)
                {
                    case "done":
                        return xmlConf.GetTitle("T_COMMON_DLP_NOTINCLUSION");            // 미포함
                    case "detected":
                        return xmlConf.GetTitle("T_COMMON_DLP_INCLUSION");            // 포함
                    default:
                        return xmlConf.GetTitle("T_COMMON_DLP_UNUSE");            // 미사용
                }
            }
            return xmlConf.GetTitle("T_COMMON_DLP_UNUSE");            // 미사용

            #region [사용안함] 고도화 이전 소스
            //string strDlp = "";
            //if (dic.TryGetValue(2, out strDlp) != true)
            //    return strDlp;
            //strDlp = dic[2];

            //int nIndex = 0;
            //if (!strDlp.Equals(""))
            //    nIndex = Convert.ToInt32(strDlp);

            //switch (nIndex)
            //{
            //    case 0:
            //        strDlp = xmlConf.GetTitle("T_COMMON_DLP_UNUSE");            // 미사용
            //        break;
            //    case 1:
            //        strDlp = xmlConf.GetTitle("T_COMMON_DLP_INCLUSION");            // 포함
            //        break;
            //    case 2:
            //        strDlp = xmlConf.GetTitle("T_COMMON_DLP_NOTINCLUSION");            // 미포함
            //        break;
            //    case 3:
            //        strDlp = xmlConf.GetTitle("T_COMMON_DLP_UNKNOWN");            // 검출불가
            //        break;
            //    default:
            //        strDlp = "0";
            //        break;
            //}
            //return strDlp; 
            #endregion
        }

        public string GetScanFileDescription(object selectedInfo, string scanType, string fileSeq)
        {
            List<object> scanList = selectedInfo.GetTagDataObjectList("scan_list");
            if (scanList?.Count < 1)
                return "-";

            foreach (object scan in scanList)
            {
                //해당 scanType 종류 검색
                if (scan.GetTagDataString("type").ToUpper() != scanType.ToUpper())
                    continue;

                //해당 파일 검출 내용 검색
                List<object> scanFiles = scan.GetTagDataObjectList("result", "scan_record_list");
                foreach (object file in scanFiles)
                {
                    if (file.GetTagDataString("file_seq") == fileSeq)
                        return file.GetTagDataString("description");
                }
            }

            return "-";
        }

        /// <summary>
        /// 반입/반출
        /// <para>"trans_req", "net_type"</para>
        /// </summary>
        /// <param name="selectedInfo"></param>
        /// <returns></returns>
        public string GetTransKind(object selectedInfo)
        {
            string netType = selectedInfo.GetTagDataString("trans_req", "net_type");
            if (netType == "I")
                return xmlConf.GetTitle("T_COMMON_IMPORT");         //반입
            else if (netType == "E")
                return xmlConf.GetTitle("T_COMMON_EXPORT");          // 반출
            else
                return "-";
        }


        /// <summary>
        /// 결재 요청자
        /// <para>"trans_req", "req_user_seq"</para>
        /// </summary>
        /// <param name="selectedInfo"></param>
        /// <returns></returns>
        public string GetApproveReqUser(object selectedInfo) => selectedInfo.GetTagDataString("trans_req", "req_user_seq");

        /// <summary>
        /// trans_req"."trans_seq
        /// </summary>
        /// <param name="selectedInfo"></param>
        /// <returns></returns>
        public string GetTransSeq(object selectedInfo) => selectedInfo.GetTagDataString("trans_req", "trans_seq");

        /**
		 * @breif ApproveSequence 정보를 반환한다.
		 * @return ApproveSequence 정보
		 */
        public string GetApproveSeq(Dictionary<int, string> dic)
        {
            string strApproveSeq = "";
            if (dic.TryGetValue(1, out strApproveSeq) != true)
                return strApproveSeq;
            strApproveSeq = dic[1];
            return strApproveSeq;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strApprStatus">
        /// <para>scanning 검사중</para>
        /// <para>wait 전송대기</para>
        /// <para>cancel 전송취소</para>
        /// <para>received PC수신완료</para>
        /// <para>fail 전송실패</para>
        /// </param>
        /// <param name="strTransStatus">
        /// <para></para>
        /// </param>
        /// <returns>요청취소 판단 값. (0 : 요청취소 조건 아님, 1: 사용자가 전송취소 한 경우, 2: 이전 결재자가 반려한 경우</returns>
        public int GetRequestCancelChk(object selectedInfo)
        {
            string strTransStatus = selectedInfo.GetTagDataString("trans_req", "trans_state");
            string strApprStatus = selectedInfo.GetTagDataString("trans_req", "approval_state");

            if (string.IsNullOrEmpty(strApprStatus) || string.IsNullOrEmpty(strTransStatus))
                return 0;
            /*
            - "scanning 검사중"
            - "wait 전송대기"
            - "cancel 전송취소"
            - "received PC수신완료"
            - "fail 전송실패"
            ------------------------------
             pre(이전 단계 진행중), wait(결재 대기), confirm(승인), reject(반려), cancel(취소), skip(결재 스킵)
            - "pre 이전 단계 진행중"
            - "wait 결재 대기"
            - "confirm 승인"
            - "reject 반려"
            - "cancel 취소"
            - "skip 결재스킵(타 결재자 처리)"
            */
            if (strTransStatus == "cancel" && strApprStatus == "wait")     // 전송상태가 전송취소이면서, 결재상태가 승인대기일때
                return 1;

            if (strTransStatus == "wait" && strApprStatus == "reject")      //전송상태가 전송대기이면서, 결재상태가 반려인 경우
                return 2;

            return 0;
        }


        public string GetTransStatusCode(object selectedInfo) => selectedInfo.GetTagDataString("trans_req", "trans_state");
        public string GetTransStatusName(object selectedInfo)
        {
            string strTransState = selectedInfo.GetTagDataString("trans_req", "trans_state");
            switch (strTransState)
            {
                case "scanning":
                    return xmlConf.GetTitle("T_COMMON_TRANSCHECKING");  //검사중
                case "wait":
                    return xmlConf.GetTitle("T_COMMON_TRANSWAIT");  //전송대기
                case "cancel":
                    return xmlConf.GetTitle("T_COMMON_TRANSCANCLE");  //전송취소
                case "received":
                    return xmlConf.GetTitle("T_TRANS_COMPLETE");  //PC수신완료
                case "fail":
                    return xmlConf.GetTitle("T_TRANS_COMPLETE");  //전송실패
                default:
                    return "-";
            }
        }


        /// <summary>
        /// 결재상태 원본 ("trans_req", "approval_state")
        /// <para>pre, wait, confirm, rejcet, skip</para>
        /// </summary>
        /// <param name="selectedInfo"></param>
        /// <returns></returns>
        public string GetApprStausCode(object selectedInfo) => selectedInfo.GetTagDataString("trans_req", "approval_state");

        /**
		 * @breif 결재 테이블 위치 정보를 반환한다.
		 * @return 결재 테이블 위치 정보(C : 결재 테이블, H : 결재 이력테이블)
		 */
        public string GetApprDataPos(Dictionary<int, string> dic)
        {
            string strApprDataPos = "";
            if (dic.TryGetValue(13, out strApprDataPos) != true)
                return strApprDataPos;

            strApprDataPos = dic[13];             // 결재테이블 위치 정보
            return strApprDataPos;
        }

        /**
		 * @breif 결재 가능 여부를 반환한다.
		 * @return 결재 가능 여부(0: 결재불가능, 1:결재가능)
		 */
        public string GetApprPossible(Dictionary<int, string> dic)
        {
            string strApprPossible = "";
            if (dic.TryGetValue(14, out strApprPossible) != true)
                return strApprPossible;

            strApprPossible = dic[14];             // 결재가능 여부

            int nIndex = 0;
            if (!strApprPossible.Equals(""))
                nIndex = Convert.ToInt32(strApprPossible);
            return strApprPossible;
        }

        /**
		 * @breif 결재자가 포함된 결재단계의 결재상태 정보를 반환한다.
		 * @return 결재자가 포함된 결재단계의 결재상태 정보(1: 결재가능, 2:결재불가능)
		 */
        public string GetApprStepPossible(Dictionary<int, string> dic)
        {
            string strApprStepPossible = "";
            if (dic.TryGetValue(15, out strApprStepPossible) != true)
                return strApprStepPossible;

            strApprStepPossible = dic[15];             // 결재자가 포함된 결재단계의 결재상태 정보

            int nIndex = 0;
            if (!strApprStepPossible.Equals(""))
                nIndex = Convert.ToInt32(strApprStepPossible);
            return strApprStepPossible;
        }

        /**
		 * @breif 결재상태 정보를 반환한다.
		 * @return 결재상태 정보(요청취소,승인대기,승인,반려)
		 */
        public string GetApprStatus(object selectedInfo)
        {
            //고도화때는, 승인상태와 전송상태가 따로 가지 않기 때문에 해당 조건 불필요
            //if (GetRequestCancelChk(dic) != 0)
            //{
            //    //전송취소 이면서 승인대기
            //    //전송대기 이지만 반려
            //    return xmlConf.GetTitle("T_COMMON_REQUESTCANCEL");       // 요청취소
            //}

            string strApproStatus = selectedInfo.GetTagDataString("trans_req", "approval_state");
            return GetApprStatusName(strApproStatus);
        }

        public string GetApprStatusName(string ApprStatusCode) => ApprStatusCode switch
        {
            "pre" or "wait" => xmlConf.GetTitle("T_COMMON_APPROVE_WAIT"),              // 승인대기
            "confirm" => xmlConf.GetTitle("T_COMMON_APPROVE"),                   // 승인
            "reject" => xmlConf.GetTitle("T_COMMON_REJECTION"),                   // 반려
            "skip" => xmlConf.GetTitle("T_COMMON_REQUESTCANCEL"),                   // 요청취소
            _ => "-"
        };


        /**
		 * @breif 파일 포워딩 사용 여부를 반환한다.
		 * @return 파일 포워딩 사용 여부 (사용, 미사용)
		 */
        public string GetUseFileForward(Dictionary<int, string> dic)
        {
            string strUseFileForward = "-";
            if (dic.TryGetValue(16, out strUseFileForward) != true)
                return strUseFileForward;

            strUseFileForward = dic[16];            // 파일 포워딩 사용 여부 ( 0 : 포워딩한 사용자가 없음, 1: 포워딩한 사용자가 있음)

            int nIndex = 0;
            if (!strUseFileForward.Equals(""))
                nIndex = Convert.ToInt32(strUseFileForward);

            switch (nIndex)
            {
                case 0:
                    strUseFileForward = xmlConf.GetTitle("T_COMMON_FORWARD_UNUSE");              // 미사용
                    break;
                case 1:
                    strUseFileForward = xmlConf.GetTitle("T_COMMON_FORWARD_USE");                   // 사용
                    break;
                default:
                    strUseFileForward = "-";
                    break;
            }
            return strUseFileForward;
        }


        /**
        * @breif 파일 수신위치 정보를 반환한다.
        * @return 파일 수신위치(보안웹하드, 업무PC/인터넷PC)
        */
        public string GetRecvPos(Dictionary<int, string> dic)
        {
            string strRecvPos = "";
            string strTransKind = "";
            if ((dic.TryGetValue(18, out strRecvPos) != true) || (dic.TryGetValue(6, out strTransKind) != true))
                return strRecvPos;

            strRecvPos = dic[18];
            strTransKind = dic[6];

            int nIndex = 0;
            if (!strRecvPos.Equals(""))
                nIndex = Convert.ToInt32(strRecvPos);

            switch (nIndex)
            {
                case 0:
                    if (strTransKind.Equals("1"))                // 반출이면
                        strRecvPos = xmlConf.GetTitle("T_RECV_INTERNETPC");     // 인터넷 PC
                    else                                        // 반입이면
                        strRecvPos = xmlConf.GetTitle("T_RECV_BUSINESSPC");     //  PC
                    break;
                case 1:
                    strRecvPos = xmlConf.GetTitle("T_SECURITY_WEBHARD");        // 보안웹하드
                    break;
                default:
                    strRecvPos = "-";
                    break;
            }
            return strRecvPos;
        }



        public string GetTitle(object selectedInfo) => selectedInfo.GetTagDataString("trans_req", "title");

        /// <summary>
        /// 설명을 반환한다.
        /// </summary>
        /// <returns></returns>
        public string GetDesc(object selectedInfo) => selectedInfo.GetTagDataString("trans_req", "description");

        /**
		 * @breif 전송요청일 정보를 반환한다.
		 * @return 전송요청일(type : YYYY-MM-DD hh:mm:ss)
		 */
        public string GetTransReqDay(Dictionary<int, string> dic)
        {
            string strTransReqDay = "";
            if (dic.TryGetValue(11, out strTransReqDay) != true)
                return strTransReqDay;

            strTransReqDay = dic[11];
            string strYear = strTransReqDay.Substring(0, 4);
            string strMonth = strTransReqDay.Substring(4, 2);
            string strDay = strTransReqDay.Substring(6, 2);
            string strHour = strTransReqDay.Substring(8, 2);
            string strMinute = strTransReqDay.Substring(10, 2);
            string strSecond = strTransReqDay.Substring(12, 2);

            strTransReqDay = String.Format("{0}-{1}-{2} {3}:{4}:{5}", strYear, strMonth, strDay, strHour, strMinute, strSecond);
            return strTransReqDay;
        }
        /**
		 * @breif 결재일 정보를 반환한다.
		 * @return 결재일(type : YYYY-MM-DD hh:mm:ss)
		 */
        public string GetApprDay(Dictionary<int, string> dic)
        {
            string strApprDay = "";
            if (dic.TryGetValue(12, out strApprDay) != true)
                return "-";

            strApprDay = dic[12];
            string strYear = strApprDay.Substring(0, 4);                // 년도
            string strMonth = strApprDay.Substring(4, 2);               // 월
            string strDay = strApprDay.Substring(6, 2);                 // 일
            string strHour = strApprDay.Substring(8, 2);                // 시각
            string strMinute = strApprDay.Substring(10, 2);             // 분
            string strSecond = strApprDay.Substring(12, 2);             // 초

            strApprDay = String.Format("{0}-{1}-{2} {3}:{4}:{5}", strYear, strMonth, strDay, strHour, strMinute, strSecond);
            return strApprDay;
        }
        //     /**
        //* @breif 전송요청일 정보를 반환한다.
        //* @return 전송요청일(type : YYYY-MM-DD hh:mm:ss)
        //*/
        /// <summary>
        /// 전송요청일 (type : YYYY-MM-DD hh:mm:ss)
        /// <para>"trans_req", "req_datetime"</para>
        /// </summary>
        /// <param name="selectedInfo"></param>
        /// <returns></returns>
        public string GetQueryTransReqDay(object selectedInfo) => selectedInfo.GetTagDataString("trans_req", "req_datetime");
        /**
		 * @breif 결재일 정보를 반환한다.
		 * @return 결재일(type : YYYY-MM-DD hh:mm:ss)
		 */
        public string GetQueryApprDay(object selectedInfo)
        {
            string strTransStatus = selectedInfo.GetTagDataString("trans_req", "trans_state");
            string strApproStatus = selectedInfo.GetTagDataString("trans_req", "approval_state");
            if (GetRequestCancelChk(selectedInfo) != 0)
                return "-";

            string strApprStatus = GetApprStatus(selectedInfo);
            string strTempApprStatus1 = xmlConf.GetTitle("T_COMMON_APPROVE");               // 승인
            string strTempApprStatus2 = xmlConf.GetTitle("T_COMMON_REJECTION");             // 반려

            if ((strApprStatus.Equals(strTempApprStatus1)) || (strApprStatus.Equals(strTempApprStatus2)))
            {
                List<object> approvalStepStatusList = selectedInfo.GetTagDataObjectList("approval_step_status_list");
                if (approvalStepStatusList?.Count <= 0)
                    return "-";

                int lastApprOrder = 0;
                string lastApprovalRespTime = "-";
                foreach (object status in approvalStepStatusList)
                {
                    string apprStat = status.GetTagDataObject("approval_state").ToString();
                    if (apprStat != "confirm" && apprStat != "reject")
                        continue;

                    string apprOrderString = status.GetTagDataObject("approval_step", "approval_order").ToString();
                    int.TryParse(apprOrderString, out int apprOrder);

                    if (lastApprOrder < apprOrder)
                    {
                        lastApprOrder = apprOrder;
                        lastApprovalRespTime = status.GetTagDataObject("resp_datetime").ToString();
                    }
                }
                return lastApprovalRespTime;    //마지막 결재자가 결재한 시간 반환
            }
            else
                return "-";
        }

        /// <summary>
        /// 해당 UserSeq의 결재자에 대한 정보 반환
        /// <para>승인/반려는 해당 결재에 대한 정보 반환</para>
        /// <para>그 외 결재상태는 상태만 반화승인대기이거나, 요청취소인 경우엔, 결재상태만 반환</para>
        /// </summary>
        /// <param name="selectedInfo"></param>
        /// <param name="approvalUserSeq"></param>
        /// <returns></returns>
        public ApprovalInfo GetApprovalInfo(object selectedInfo, string approvalUserSeq)
        {
            ApprovalInfo retValue = new ApprovalInfo("-", "-", "-", "-", "-", "-", "-");

            //TODO 고도화 - 대결재자인 경우, 원결재자를 반환하도록 처리 필요
            List<object> approvalStepStatusList = selectedInfo.GetTagDataObjectList("approval_step_status_list");
            if (approvalStepStatusList?.Count <= 0)
                return retValue;

            foreach (object status in approvalStepStatusList)
            {
                string apprSeq = status.GetTagDataString("approver_hr", "approver_seq");
                string apprType = status.GetTagDataString("approval_step", "approval_type");

                if (approvalUserSeq != apprSeq)
                    continue;

                if (DisplayUIType == UIType.Common && apprType != "common" )    //결재라인 중, 일반 결재타입의 사용자 중 조회
                    continue;

                if (DisplayUIType == UIType.Clipboard&& apprType != "common")    //결재라인 중, 일반 결재타입의 사용자 중 조회
                    continue;

                if (DisplayUIType == UIType.Security && apprType != "security")    //결재라인 중, 보안 결재타입의 사용자 중 조회
                    continue;

                string apprStat = status.GetTagDataString("approval_state");
                if (apprStat == "confirm" || apprStat == "reject")  //승인,반려일때는 모든 정보 반환
                {
                    retValue.UserSeq = apprSeq;
                    retValue.UserID = status.GetTagDataString("approver_hr", "approver_id");
                    retValue.UserName = status.GetTagDataString("approval_step", "approval_name");
                    retValue.ApproveStatusCode = apprStat;
                    retValue.ApproveStatusName = GetApprStatusName(apprStat);
                    retValue.ApproveReason = status.GetTagDataString("description");
                    retValue.ApproveTime = status.GetTagDataString("resp_datetime");
                    return retValue;
                }
                else
                {
                    retValue.ApproveStatusCode = apprStat;
                    retValue.ApproveStatusName = GetApprStatusName(apprStat);
                    return retValue;
                }

            }
            return retValue;
        }

        /// <summary>
        /// 마지막 승인대기 결재자 정보 반환
        /// <para>전송상태나, 결재상태로 보아 결재가 가능한 상태일때만 정보 반환</para>
        /// </summary>
        /// <param name="selectedInfo"></param>
        /// <returns></returns>
        public ApprovalInfo GetLastApprInfo(object selectedInfo)
        {
            ApprovalInfo retValue = new ApprovalInfo("-", "-", "-", "-", "-", "-", "-");

            if (GetRequestCancelChk(selectedInfo) != 0)
                return retValue;

            string strApprStatus = GetApprStatus(selectedInfo);
            string strTempApprStatus1 = xmlConf.GetTitle("T_COMMON_APPROVE");               // 승인
            string strTempApprStatus2 = xmlConf.GetTitle("T_COMMON_REJECTION");             // 반려

            //TODO 고도화 - 대결재자인 경우, 원결재자를 반환하도록 처리 필요
            if ((strApprStatus.Equals(strTempApprStatus1)) || (strApprStatus.Equals(strTempApprStatus2)))
            {
                List<object> approvalStepStatusList = selectedInfo.GetTagDataObjectList("approval_step_status_list");
                if (approvalStepStatusList?.Count <= 0)
                    return retValue;

                int lastApprOrder = 0;
                foreach (object status in approvalStepStatusList)
                {
                    string apprStat = status.GetTagDataString("approval_state");
                    if (apprStat != "confirm" && apprStat != "reject")
                        continue;

                    string apprOrderString = status.GetTagDataString("approval_step", "approval_order");
                    int.TryParse(apprOrderString, out int apprOrder);

                    if (lastApprOrder < apprOrder)
                    {
                        lastApprOrder = apprOrder;
                        retValue.ApproveStatusCode = apprStat;
                        retValue.ApproveStatusName = GetApprStatusName(apprStat);
                        retValue.UserID = status.GetTagDataString("approver_hr", "approver_id");
                        retValue.UserName = status.GetTagDataString("approval_step", "approval_name");
                        retValue.UserSeq = status.GetTagDataString("approval_step", "approver_seq");
                        retValue.ApproveReason = status.GetTagDataString("description");
                        retValue.ApproveTime = status.GetTagDataString("resp_datetime");
                    }
                }
                return retValue;    //마지막 결재자 반환
            }
            else
                return retValue;
        }

        /// <summary>
        /// 결재자 이름 정보 반환 (결재한 마지막 승인/반려 결재자 반환)
        /// </summary>
        /// <param name="selectedInfo"></param>
        /// <returns></returns>
        public string GetQueryApprName(object selectedInfo)
        {
            if (GetRequestCancelChk(selectedInfo) != 0)
                return "-";

            string strApprStatus = GetApprStatus(selectedInfo);
            string strTempApprStatus1 = xmlConf.GetTitle("T_COMMON_APPROVE");               // 승인
            string strTempApprStatus2 = xmlConf.GetTitle("T_COMMON_REJECTION");             // 반려


            if ((strApprStatus.Equals(strTempApprStatus1)) || (strApprStatus.Equals(strTempApprStatus2)))
            {
                List<object> approvalStepStatusList = selectedInfo.GetTagDataObjectList("approval_step_status_list");
                if (approvalStepStatusList?.Count <= 0)
                    return "-";

                int lastApprOrder = 0;
                string lastApprovalName = "-";
                foreach (object status in approvalStepStatusList)
                {
                    string apprStat = status.GetTagDataObject("approval_state").ToString();
                    if (apprStat != "confirm" && apprStat != "reject")
                        continue;

                    string apprOrderString = status.GetTagDataObject("approval_step", "approval_order").ToString();
                    int.TryParse(apprOrderString, out int apprOrder);

                    if (lastApprOrder < apprOrder)
                    {
                        lastApprOrder = apprOrder;
                        lastApprovalName = status.GetTagDataObject("approval_step", "approval_name").ToString();
                    }
                }
                return lastApprovalName;    //마지막 결재자 반환
            }
            else
                return "-";
        }
        /// <summary>
        /// 데이타 타입 결과 리턴
        /// </summary>
        /// <param name="selectedInfo"></param>
        /// <returns></returns>
        public string GetDataType(object selectedInfo)
        {
            string strDataType = selectedInfo.GetTagDataString("trans_req", "data_type");
            switch (strDataType)
            {
                case "cliptxt":
                    return xmlConf.GetTitle("T_DATA_TYPE_TEXT");
                case "clipimg":
                    return xmlConf.GetTitle("T_DATA_TYPE_IMAGE");
                default:
                    return "";
            }
        }

        /**
        * @breif 리스트 아이템의 결재 가능 여부를 판별한다.
        * @param strTransStatusCode : 전송상태 원본 코드 (W:전송대기,C:전송취소,P:전송완료,F:전송실패,V:검사중)
        * @param strApprStatusCode : 결재상태 원본코드 (1:승인대기,2:승인,3:반려)
        * @param strApprPossible : 결재 가능 불가능 (0 : 불가능 , 1: 가능)
        * @param strApprStepStatus : 결재자가 포함된 결재단계의 결재상태 정보(1: 결재가능, 2:결재불가능)
        * @return 결재 가능 여부( true : 가능, false : 불가능)
        */
        public static bool GetApprEnableChk(string strTransStatusCode, string strApprStatusCode, string strApprPossible, string strApprStepStatus)
        {
            if (strApprStatusCode.Equals("1"))                           // 승인대기
            {
                if (strApprPossible.Equals("0"))                         // 승인불가능
                    return false;
                else if (strTransStatusCode.Equals("V") == true)
                {
                    return false;
                }
                else
                {
                    if ((strApprStatusCode.Equals("1") == true) && (strApprStepStatus.Equals("2") == true))         // 승인대기 이지만 strApprStepStatus 값이 결재 불가능일 때
                        return false;
                    else
                    {
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
        }


        /**
        * @breif 리스트 아이템의 승인 대기 여부를 판별한다.
        * @return 승인대기 여부( true : 승인대기, false : 승인대기 아님)
        */
        public bool GetApproveWait(Dictionary<int, string> dic)
        {
            string strTransStatusCode = "";
            string strApprKind = "";
            string strApprStatusCode = "";
            string strApprDataPos = "";


            if (
                (dic.TryGetValue(7, out strTransStatusCode) != true)
                || (dic.TryGetValue(8, out strApprKind) != true)
                || (dic.TryGetValue(9, out strApprStatusCode) != true)
                || (dic.TryGetValue(13, out strApprDataPos) != true)
                )
                return false;

            strTransStatusCode = dic[7];
            strApprKind = dic[8];
            strApprStatusCode = dic[9];
            strApprDataPos = dic[13];

            if ((strApprStatusCode.Equals("1"))
                && (!strTransStatusCode.Equals("C"))
                && (!strTransStatusCode.Equals("F"))
                )
            {
                if ((strApprKind.Equals("0")) && (strApprDataPos.Equals("H")))
                    return false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 결재 가능 여부를 반환
        /// <para>전송 건에 대한 결재 가능 상태 + userSeq의 순서가 모두 적합하면 true 반환</para>
        /// </summary>
        /// <param name="userSeq"></param>
        /// <param name="selectedInfo"></param>
        /// <returns></returns>
        public bool GetApprEnableChk(string userSeq, object selectedInfo)
        {
            /*
             * [trans_state]
              - "scanning 검사중"
              - "wait 전송대기"
              - "cancel 전송취소"
              - "received PC수신완료"
              - "fail 전송실패"
              ------------------------------
               [approval_state]
              - "pre 이전 단계 진행중"
              - "wait 결재 대기"
              - "confirm 승인"
              - "reject 반려"
              - "cancel 취소"
              - "skip 결재스킵(타 결재자 처리)"
              */

            if (GetRequestCancelChk(selectedInfo) != 0)
                return false;

            string strApproStatus = selectedInfo.GetTagDataString("trans_req", "approval_state");
            if (strApproStatus != "wait")
                return false;

            //승인대기 이지만, Step 계산 시, 해당 결재자가 결재할 단계가 아닌 경우, False
            List<object> approvalStepStatusList = selectedInfo.GetTagDataObjectList("approval_step_status_list");
            if (approvalStepStatusList?.Count <= 0)
                return false;

            foreach (object status in approvalStepStatusList)
            {
                if (status.GetTagDataObject("approval_state").ToString() == "wait" &&
                    status.GetTagDataObject("approval_step", "approval_seq").ToString() == userSeq)
                {
                    //UserSeq가 현재 결재해야할 order에 포함되어 있는지 여부
                    return true;
                }
            }
            return false;
        }



        /**
        * @breif 선택된 리스트 아이템의 승인 또는 반려 가능 여부를 판별한다.
        * @param bApprAction 승인 가능 여부(out)
        * @param bApprReject 반려 가능 여부(out)
        * @param strTransStatusCode : 전송상태 원본 코드 (W:전송대기,C:전송취소,P:전송완료,F:전송실패,V:검사중)
        * @param strApprStatusCode : 결재상태 원본코드 (1:승인대기,2:승인,3:반려)
        * @param strApprPossible : 결재 가능 불가능 (0 : 불가능 , 1: 가능)
        * @param strApprStepStatus : 결재자가 포함된 결재단계의 결재상태 정보(1: 결재가능, 2:결재불가능)
        * @return 결재 가능 여부( true : 가능, false : 불가능)
        */
        public static void GetApprActionRejectChk(out bool bApprAction, out bool bApprReject, string strTransStatusCode, string strApprStatusCode, string strApprPossible, string strApprStepStatus)
        {
            if (strApprStatusCode.Equals("1"))                           // 승인대기
            {
                if (strApprPossible.Equals("0"))                         // 승인불가능
                {
                    bApprAction = bApprReject = false;
                }
                else
                {
                    if ((strApprStatusCode.Equals("1") == true) && (strApprStepStatus.Equals("2") == true))         // 승인대기 이지만 strApprStepStatus 값이 결재 불가능일 때
                    {
                        bApprAction = bApprReject = false;
                    }
                    else
                    {
                        bApprAction = true;
                        if (strApprPossible.Equals("2"))
                            bApprReject = false;
                        else
                            bApprReject = true;
                    }
                }
                if (strTransStatusCode.Equals("V"))
                {
                    bApprAction = bApprReject = false;
                }
            }
            else
            {
                bApprAction = bApprReject = false;
            }

            return;
        }

        /// <summary>
        /// 목적지망 정보 ("destination", "destination", "sg_net_type_list")
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetDestNetworkName(object selectedInfo, Dictionary<string, SGNetOverData> destInfo)
        {
            List<string> retValue = new List<string>();
            List<object> TransferDestObject = selectedInfo.GetTagDataObjectList("destination", "destination", "sg_net_type_list");

            if (TransferDestObject?.Count > 0)
            {
                List<string> TransferDest = TransferDestObject.Select(i => i.ToString()).ToList();
                foreach (SGNetOverData dest in destInfo?.Values)
                {
                    if (TransferDest.Contains(dest.strDestSysid))
                        retValue.Add(dest.strDestSysName);
                }
            }
            return string.Join(",", retValue);
        }

        /// <summary>
        /// 전송상태에 따라 해당 Transfer건의 서버 위치 Name 반환
        /// </summary>
        /// <param name="selectedInfo"></param>
        /// <returns></returns>
        public string GetFilePos(object selectedInfo, List<SGNetOverData> UserSgNetAllList)
        {
            /*
             사후결재 - 
	            검사중 Src 서버
	            PC수신대기중 Dest 서버
	            PC수신완료 Dest 서버
            사전결재 - 
	            ~승인대기 Src 서버
	            ~ Dest 서버             
             */
            string ProcessType = selectedInfo.GetTagDataString("trans_req", "approval_proc_type"); //pre,post
            string TransStatus = selectedInfo.GetTagDataString("trans_req", "trans_state"); //scanning,wait,cancel,ready_to_receive, received,fail
            string SrcNetType = selectedInfo.GetTagDataString("trans_req", "source_sg_net_type");
            List<object> DestNetType = selectedInfo.GetTagDataObjectList("destination", "sg_net_type_list");

            List<string> position = new List<string>();   //src, dest[]
            if (ProcessType == "pre")    //사전
            {
                switch (TransStatus)
                {
                    case "scanning":            //검사중
                    case "wait":                //전송대기
                    case "cancel":              //전송취소
                    case "fail":                //전송실패
                        position.Add(SrcNetType);
                        break;
                    case "ready_to_receive":    //PC수신대기
                    case "received":            //PC수신완료
                        position.AddRange(DestNetType.Select(i => i.ToString()));
                        break;
                }
            }
            else if (ProcessType == "post")     //사후
            {
                switch (TransStatus)
                {
                    case "scanning":            //검사중
                    case "cancel":              //전송취소
                    case "fail":                //전송실패
                        position.Add(SrcNetType);
                        break;
                    case "wait":                //전송대기
                    case "ready_to_receive":    //PC수신대기
                    case "received":            //PC수신완료
                        position.AddRange(DestNetType.Select(i => i.ToString()));
                        break;
                }
            }

            //sgType Name 반환
            foreach (SGNetOverData sgNet in UserSgNetAllList)
            {
                if (position.Contains(sgNet.strDestSysid))
                    return sgNet.strDestSysName;
            }
            return "-";
        }


        /// <summary>
        /// 승인 요청자 정보를 반환한다.
        /// </summary>
        /// <returns></returns>
        public string GetReqUser(object selectedInfo)
        {
            string strReqUserName = selectedInfo.GetTagDataString("trans_req", "req_user_seq");                // 결재 요청자 이름
            string strReqUserPos = selectedInfo.GetTagDataString("trans_req", "req_user_hr", "rank");               // 결재 요청자 직위

            if (strReqUserName.Equals(""))
                return strReqUserName;

            strReqUserName = strReqUserName + " " + strReqUserPos;
            return strReqUserName;
        }


        /// <summary>
        /// 승인요청일 정보를 반환 (2024-01-15 12:03:30)
        /// <para>"trans_req", "req_datetime"</para>
        /// </summary>
        /// <returns></returns>
        public string GetApprReqDay(object selectedInfo) => selectedInfo.GetTagDataString("trans_req", "req_datetime");


        /// <summary>
        /// FileKey 정보를 반환한다.
        /// </summary>
        /// <returns></returns>
        public string GetFileKey(object selectedInfo) => selectedInfo.GetTagDataString("file_info", "file_key");

        /// <summary>
        /// 파일미리보기 가능 여부를 반환한다.(해당 trans의 source_sg_net_type이 접속 서버의 sg_net_type과 동일 여부)
        /// </summary>
        /// <param name="bInner"></param>
        /// <returns></returns>
        public bool GetFilePrevEnable(string userSeq, string MySgNet, object selectedInfo)
        {
            //if (m_bApprDetail != true)                      // 결재 상세보기가 아닐경우
            //    return false;

            if (GetApprEnableChk(userSeq, selectedInfo) != true)                         // 결재가 불가능한 경우
                return false;

            //전송 시작sgType이 현재 내가 접속한 sgType인 경우에만 미리보기 가능
            string sourceSgNet = selectedInfo.GetTagDataString("trans_req", "source_sg_net_type");

            if (string.IsNullOrEmpty(MySgNet) || string.IsNullOrEmpty(sourceSgNet))
                return false;

            return (MySgNet == sourceSgNet);
        }


        public void GetFileInfo(object selectedInfo, out List<FileInfoData> fileListInfo)
        {
            //List<Dictionary<int, string>> listDicdata = GetRecordData("FILERECORD");
            List<object> fileRecordList = selectedInfo.GetTagDataObjectList("file_info", "file_record_list");
            List<object> scanResultList = selectedInfo.GetTagDataObjectList("scan_result");

            if (fileRecordList == null)
            {
                fileListInfo = null;
                HsLog.err($"GetFileInfo, FILERECORD null !");
                return;
            }
            if (fileRecordList.Count <= 0)
            {
                fileListInfo = null;
                return;
            }
            string strFileName = "-";
            string strFileType = "-";
            string strFileSize = "-";
            string strVirus = "-";
            string strVirusExamDay = "-";
            string stDLP = "";
            string stDLPDesc = "";
            string strVirusFlag = "";

            List<FileInfoData> m_ListData = new List<FileInfoData>();
            foreach (object record in fileRecordList)
            {
                //TODO 고도화 - DLP 및 Prework에 대한 정의 필요
                //DLP 포함여부(1:포함)
                stDLP = record.GetTagDataObject("dlp").ToString();

                //DLP DESC
                stDLPDesc = GetScanFileDescription(selectedInfo, "DLP", record.GetTagDataString("file_seq"));
                if (string.IsNullOrEmpty(stDLPDesc))
                    stDLPDesc = "-";

                // 파일이름 
                strFileName = record.GetTagDataObject("file_name").ToString();
                if (string.IsNullOrEmpty(strFileName))
                    strFileName = "-";
                else
                    strFileName = GetFileRename(false, strFileName);

                // 파일 유형 
                strFileType = record.GetTagDataObject("file_type").ToString(); // NetLink호환 : 언어별로 다 찾아서 넣어줘야함, dir이라는걸 알 수 있는 값이 없음
                if (string.IsNullOrEmpty(strFileType.Trim()))
                    strFileType = "-";

                if (strFileType.ToUpper().Equals("DIR"))
                {
                    int index = -1;
                    index = strFileName.LastIndexOf("\\");
                    if (index >= 0)
                    {
                        string strTemp = strFileName.Substring(0, index + 1);
                        string strTemp2 = strFileName.Replace(strTemp, "");
                        if (!strFileName.Equals("\\"))
                            strFileName = strFileName.Replace(strTemp, "");
                    }
                }
                else
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        strFileName = strFileName.Replace("/", "\\");
                    }
                    else
                    {
                        strFileName = strFileName.Replace("\\", "/");
                    }
                    strFileName = System.IO.Path.GetFileName(strFileName);
                }

                // 파일 Size
                strFileSize = record.GetTagDataObject("file_size").ToString();
                if (string.IsNullOrEmpty(strFileSize))
                    strFileSize = "-";
                else
                {
                    Int64 nSize = Convert.ToInt64(strFileSize);
                    strFileSize = GetSizeStr(nSize);
                }

                // 바이러스 내역
                //TODO 고도화 - scan_record_list 확립 후 수정
                strVirus = "";

                // 바이러스 검사일 
                strVirusExamDay = "-";

                // VIRUSFLAG : VIRUS 및 파일 위변조 검출여부
                strVirusFlag = "0";

                string strFileSeq = record.GetTagDataObject("file_seq").ToString();
                m_ListData.Add(new FileInfoData(strFileName, strFileType, strFileSize, strVirus, strVirusExamDay, strFileSeq, stDLP, stDLPDesc, strVirusFlag));
            }
            fileListInfo = m_ListData;
            return;
        }

        public string GetFileRename(bool bMode, string strFileName) => SgExtFunc.hsFileRename(bMode, strFileName);

        /// <summary>
        /// 숫자를 입력받아 파일크기를 문자열로 줌
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public string GetSizeStr(long size)
        {
            string rtn = "";
            if (size == 0)
            {
                rtn = "0 KB";
                return rtn;
            }

            rtn = CsFunction.GetSizeStr(size);
            return rtn;
        }
        /// <summary>
        /// 해당 파일의 크기가 0kb 인지 확인(true: 0kb이거나 다운불가능, false: 다운가능)
        /// </summary>
        /// <param name="selectedInfo"></param>
        /// <param name="fileSeq"></param>
        /// <returns></returns>
        public bool GetFileSizeEmpty(object selectedInfo, string fileSeq)
        {
            //List<Dictionary<int, string>> listDicdata = GetRecordData("FILERECORD");
            List<object> fileRecordList = selectedInfo.GetTagDataObjectList("file_info", "file_record_list");

            if (fileRecordList?.Count <= 0)
                return true;

            foreach (object record in fileRecordList)
            {
                if (record.GetTagDataString("file_seq") == fileSeq)
                {
                    string strFileSize = record.GetTagDataString("file_size");
                    Log.Logger.Here().Information($"FILE({fileSeq}) Size : {strFileSize}");
                    Int64 nSize = 0;
                    if (!strFileSize.Equals(""))
                        nSize = Convert.ToInt64(strFileSize);

                    return ((nSize > 0) ? false : true);
                }
            }
            Log.Logger.Here().Information($"FILE Not Found !!!");
            return true;
        }

        /// <summary>
        /// 결재 이력 정보를 반환한다
        /// </summary>
        /// <returns></returns>
        public List<ApproverHist> GetApproverInfoHist(object selectedInfo, bool isVisibleApproveReason)
        {
            List<ApproverHist> approverHist = new List<ApproverHist>();
            ApproverHist tmpApprover;

            List<object> apprStepList = selectedInfo.GetTagDataObjectList("approval_step_status_list");
            if (apprStepList == null)
                return null;

            if (apprStepList.Count <= 0)
                return null;

            string strApprUserID = "";                // 결재자 ID
            string strApprName = "";                // 결재자 이름 
            string strApprPos = "";                 // 결재자 직급 및 직위
            string strApprDate = "";                // 결재일 
            string strApprStatus = "";              // 결재 상태
            string strApprReason = "";              // 반려사유
            string strApprStep = "";                 // 결재 Step
            string strPrivacyAppr = "";             // 보안결재자 여부.

            string strPreApprStatusCode = "";           // 이전 결재상태
            string strApprStatusCode = "";              // 결재 상태 원본코드
            foreach (object apprStep in apprStepList)
            {
                // 결재자 ID 
                strApprUserID = apprStep.GetTagDataObject("approver_hr", "approver_id").ToString();
                if (string.IsNullOrEmpty(strApprUserID))
                    strApprUserID = "-";


                // 결재자 이름
                strApprName = apprStep.GetTagDataObject("approver_hr", "name").ToString();
                if (string.IsNullOrEmpty(strApprName))
                    strApprName = "-";


                // 결재자 직급 및 직위
                strApprPos = apprStep.GetTagDataObject("approver_hr", "rank").ToString();
                if (string.IsNullOrEmpty(strApprPos))
                    strApprPos = "-";

                // 결재일
                strApprDate = apprStep.GetTagDataObject("resp_datetime").ToString();
                if (string.IsNullOrEmpty(strApprDate))
                    strApprDate = "-";

                // 결재 상태
                strApprStatus = apprStep.GetTagDataObject("approval_state").ToString();
                if (string.IsNullOrEmpty(strApprStatus))
                    strApprStatus = "-";

                // 반려 사유
                strApprReason = apprStep.GetTagDataObject("description").ToString();
                if (string.IsNullOrEmpty(strApprReason))
                    strApprReason = "-";

                // 결재 Step
                strApprStep = apprStep.GetTagDataObject("approval_step", "approval_order").ToString();
                if (string.IsNullOrEmpty(strApprStep))
                    strApprStep = "-";

                // 승인자 정보 (이름 + 직위)
                strApprName = strApprName + " " + strApprPos;        // 승인자 정보 (이름 + 직위)                


                //보안결재에 대한 계산 여부 불필요
                //if (Dictemp.TryGetValue(7, out strPrivacyAppr))      // 보안결재자 여부.
                //    strPrivacyAppr = Dictemp[7];
                //else
                strPrivacyAppr = "-";

                switch (strApprStatus)
                {
                    case "pre":
                    case "wait":
                        strApprStatus = xmlConf.GetTitle("T_COMMON_APPROVE_WAIT");              // 승인대기
                        break;
                    case "confirm":
                        strApprStatus = xmlConf.GetTitle("T_COMMON_APPROVE");                   // 승인
                        break;
                    case "reject":
                        strApprStatus = xmlConf.GetTitle("T_COMMON_REJECTION");                 // 반려
                        break;
                    case "skip":
                        strApprStatus = xmlConf.GetTitle("T_COMMON_REQUESTCANCEL");             //요청 취소
                        break;
                    default:
                        strApprStatus = "-";
                        break;
                }


                tmpApprover = new ApproverHist();
                tmpApprover.SetData(strApprUserID, strApprName, strApprStatus, strApprDate, strApprReason, strApprStep);
                if (strApprStep != "-")
                    tmpApprover.m_nApprStep = int.Parse(strApprStep);
                else
                    tmpApprover.m_nApprStep = 0;
                approverHist.Add(tmpApprover);
            }

            return approverHist;
        }

        public List<(string Name, string Position, string DeptName)> GetForwardUserInfo(object selectedInfo)
        {
            List<(string Name, string Position, string DeptName)> retValue = new List<(string Name, string Position, string DeptName)>();

            List<object> forwardList = selectedInfo.GetTagDataObjectList("forward_user_list");
            if (forwardList?.Count <= 0)
                return retValue;

            foreach (object forward in forwardList)
            {
                string name = forward.GetTagDataString("forward_hr", "name");
                string position = forward.GetTagDataString("forward_hr", "position");
                string dept = forward.GetTagDataString("forward_hr", "dept_name");
                retValue.Add((name, position, dept));
            }
            return retValue;
        }

    }
}
