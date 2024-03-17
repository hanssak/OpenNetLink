using System;
using System.Collections.Generic;
using System.Text;
using HsNetWorkSGData;
using HsNetWorkSG;
using OpenNetLinkApp.Services;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    public enum eApprManageMsg
    {
        eNone = 0,
        eNotData = 1,
        eSearchError = 2,
        eApprBatchError = 3,
        eApprBatchActionSuccess = 4,
        eApprBatchRejectSuccess = 5
    }
    public class SGApprManageData : SGData
    {
        XmlConfService xmlConf;
        public SGApprManageData()
        {
            xmlConf = new XmlConfService();
        }

        ~SGApprManageData()
        {

        }
        override public void Copy(HsNetWork hs, SGData data)
        {
            SetProtectedSessionKey(hs.GetProtectedSeedKey());
            m_DicTagData = new Dictionary<string, object>(data.m_DicTagData);
            m_DicRecordData = new List<Dictionary<int, string>>(data.m_DicRecordData);
        }
        public int GetSearchResultCount()
        {
            string strData = GetTagData("APPROVECOUNT");
            int count = 0;
            if (!strData.Equals(""))
                count = Convert.ToInt32(strData);
            return count;
        }
        public static string ReturnMessage(eApprManageMsg eType)
        {
            string strMsg = "";
            XmlConfService xmlConf = new XmlConfService();
            switch (eType)
            {
                case eApprManageMsg.eNone:
                    strMsg = "";
                    break;
                case eApprManageMsg.eNotData:
                    strMsg = xmlConf.GetWarnMsg("W_0242");   // 검색 결과가 존재하지 않습니다.
                    break;
                case eApprManageMsg.eSearchError:
                    strMsg = xmlConf.GetErrMsg("E_0205");       // 검색 요청 중 오류가 발생되었습니다.
                    break;
                case eApprManageMsg.eApprBatchError:
                    strMsg = xmlConf.GetErrMsg("E_0207");       // 결재 요청 중 오류가 발생되었습니다.
                    break;
                case eApprManageMsg.eApprBatchActionSuccess:
                    strMsg = xmlConf.GetInfoMsg("I_0034");       // 승인이 완료되었습니다.
                    break;
                case eApprManageMsg.eApprBatchRejectSuccess:
                    strMsg = xmlConf.GetInfoMsg("I_0017");       // 반려가 완료되었습니다.
                    break;
            }

            return strMsg;
        }
        public List<Dictionary<int, string>> GetSearchData()
        {
            List<Dictionary<int, string>> listDicdata = GetRecordData("APPROVERECORD");
            return listDicdata;
        }

        /// <summary>
        /// 삭제예정
        /// </summary>
        /// <returns></returns>
        public List<Dictionary<int, string>> GetQuerySearchData()
        {
            List<Dictionary<int, string>> listDicdata = null;
            listDicdata = GetSvrRecordData("RECORD");
            if (listDicdata == null)
                return null;

            int dataCount = listDicdata.Count;
            if (dataCount <= 0)
                return null;
            return listDicdata;
        }

        public int GetTotalPageCount()
        {
            string page = GetTagData("total_page_count");
            if (int. TryParse(page, out int retValue) == false)
                return 1;
            else 
                return retValue;
        }


        /// <summary>
        /// 결재 정보(선결,후결)
        /// <para>"trans_req", "approval_proc_type"</para>
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetApprKind(Dictionary<string, object> dic)
        {
            string processType = dic.GetTagData("trans_req", "approval_proc_type");
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
        public string GetDlp(Dictionary<string, object> dic)
        {
            //Todo 고도화 - 개인정보 검출(또는 검사결과에 대한 정보) 필요함.
            return "검사결과";

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

        /// <summary>
        /// 반입/반출
        /// <para>"trans_req", "net_type"</para>
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetTransKind(Dictionary<string, object> dic)
        {
            string netType = dic.GetTagData("trans_req", "net_type");
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
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetApproveReqUser(Dictionary<string, object> dic) => dic.GetTagData("trans_req", "req_user_seq");

        /// <summary>
        /// trans_req"."trans_seq
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetTransSeq(Dictionary<string, object> dic) => dic.GetTagData("trans_req", "trans_seq");
    
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
        public int GetRequestCancelChk(Dictionary<string, object> dic)
        {
            string strTransStatus = dic.GetTagData("trans_req", "trans_state");
            string strApprStatus = dic.GetTagData("trans_req", "approval_state");

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
            if (strTransStatus == "cancel" && strApprStatus == "wait")     // 전송상태가 전송취소이면서 결재상태가 승인대기일때
                return 1;

            if (strTransStatus == "wait" && strApprStatus == "reject")      //전송대기 이면서, 반려상태인 경우
                return 2;

            return 0;
        }
        //     /**
        //* @breif 전송상태 원본데이터 정보를 반환한다.
        //* @return 전송상태 원본데이터(C : 전송취소, W : 전송대기, S : 수신완료, F : 전송실패, V : 검사중)
        //*/

        public string GetTransStatusCode(Dictionary<string, object> dic) => dic.GetTagData("trans_req", "trans_state");
        public string GetTransStatusName(Dictionary<string, object> dic)
        {
            string strTransState = dic.GetTagData("trans_req", "trans_state");
            switch(strTransState)
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
        //     /**
        //* @breif 결재상태 원본 데이터 정보를 반환한다.
        //* @return 결재상태 원본 데이터(1: 승인대기, 2:승인, 3: 반려)
        //*/

        /// <summary>
        /// 결재상태 원본 ("trans_req", "approval_state")
        /// <para>pre, wait, confirm, rejcet, skip</para>
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetApprStausCode(Dictionary<string, object> dic) => dic.GetTagData("trans_req", "approval_state");

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
        public string GetApprStaus(Dictionary<string, object> dic)
        {
            if (GetRequestCancelChk(dic) != 0)
            {
                //전송취소 이면서 승인대기
                //전송대기 이지만 반려
                return xmlConf.GetTitle("T_COMMON_REQUESTCANCEL");       // 요청취소
            }

            string strApproStatus = dic.GetTagData("trans_req", "approval_state");
            switch (strApproStatus)
            {
                case "pre":
                case "wait":
                    return xmlConf.GetTitle("T_COMMON_APPROVE_WAIT");              // 승인대기
                case "confirm":
                    return xmlConf.GetTitle("T_COMMON_APPROVE");                   // 승인
                case "reject":
                    return xmlConf.GetTitle("T_COMMON_REJECTION");                   // 반려
                case "skip":
                    return xmlConf.GetTitle("T_COMMON_REQUESTCANCEL");                   // 요청취소
                default:
                    return "-";
            }
        }

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


        public string GetTitle(Dictionary<string, object> dic) => dic.GetTagData("trans_req", "title");
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
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetQueryTransReqDay(Dictionary<string, object> dic) => dic.GetTagData("trans_req", "req_datetime");
        /**
		 * @breif 결재일 정보를 반환한다.
		 * @return 결재일(type : YYYY-MM-DD hh:mm:ss)
		 */
        public string GetQueryApprDay(Dictionary<string, object> dic)
        {
            string strTransStatus = dic.GetTagData("trans_req", "trans_state");
            string strApproStatus = dic.GetTagData("trans_req", "approval_state");
            if (GetRequestCancelChk(dic) != 0)
                return "-";

            string strApprStatus = GetApprStaus(dic);
            string strTempApprStatus1 = xmlConf.GetTitle("T_COMMON_APPROVE");               // 승인
            string strTempApprStatus2 = xmlConf.GetTitle("T_COMMON_REJECTION");             // 반려

            if ((strApprStatus.Equals(strTempApprStatus1)) || (strApprStatus.Equals(strTempApprStatus2)))
            {
                List<object> approvalStepStatusList = dic.GetTagDataObjectList("approval_step_status_list");
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
        /// 결재자 정보 반환 (결재한 마지막 승인/반려 결재자 반환)
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetQueryApprName(Dictionary<string, object> dic)
        {
            if (GetRequestCancelChk(dic) != 0)
                return "-";

            string strApprStatus = GetApprStaus(dic);
            string strTempApprStatus1 = xmlConf.GetTitle("T_COMMON_APPROVE");               // 승인
            string strTempApprStatus2 = xmlConf.GetTitle("T_COMMON_REJECTION");             // 반려

            
            if ((strApprStatus.Equals(strTempApprStatus1)) || (strApprStatus.Equals(strTempApprStatus2)))
            {
                List<object> approvalStepStatusList = dic.GetTagDataObjectList("approval_step_status_list");
                if (approvalStepStatusList?.Count <= 0)
                    return "-";

                int lastApprOrder =0;
                string lastApprovalName = "-";
                foreach (object status in approvalStepStatusList)
                {
                    string apprStat = status.GetTagDataObject("approval_state").ToString();
                    if (apprStat != "confirm" && apprStat != "reject")
                        continue;

                    string apprOrderString = status.GetTagDataObject("approval_step", "approval_order").ToString();
                    int.TryParse(apprOrderString, out int apprOrder);

                    if(lastApprOrder < apprOrder)
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
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetDataType(Dictionary<string, object> dic)
        {
            string strDataType = dic.GetTagData("trans_req", "data_type");         
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
        public static bool GetApprEnableChk(string strTransStatusCode, string strApprStatusCode,string strApprPossible, string strApprStepStatus)
        {
            if(strApprStatusCode.Equals("1"))                           // 승인대기
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
        /**
        * @breif 리스트 아이템의 결재 가능 여부를 판별한다.
        * @return 결재 가능 여부( true : 가능, false : 불가능)
        */
        public bool GetApprEnableChk(string userSeq,  Dictionary<string, object> dic)
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
            
            if (GetRequestCancelChk(dic) != 0)
                return false;

            string strApproStatus = dic.GetTagData("trans_req", "approval_state");
            if (strApproStatus != "wait")
                return false;

            //승인대기 이지만, Step 계산 시, 해당 결재자가 결재할 단계가 아닌 경우, False
            List<object> approvalStepStatusList = dic.GetTagDataObjectList("approval_step_status_list");
            if (approvalStepStatusList?.Count <= 0)
                return false;
            
            foreach(object status in approvalStepStatusList)
            {
                if(status.GetTagDataObject("approval_state").ToString() == "wait" && 
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
        public string GetDestNetworkName(Dictionary<string, object> dic, Dictionary<string, SGNetOverData> destInfo)
        {
            List<string> retValue = new List<string>();
            List<string> TransferDest = dic.GetTagDataList("destination", "destination", "sg_net_type_list");

            foreach(SGNetOverData dest in destInfo?.Values)
            {
                if (TransferDest.Contains(dest.strDestSysid))
                    retValue.Add(dest.strDestSysName);
            }

            return string.Join(",", retValue);            
        }
        

    }
}
